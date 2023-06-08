using AggieEnterpriseApi;
using AggieEnterpriseApi.Extensions;
using AggieEnterpriseApi.Types;
using AggieEnterpriseApi.Validation;
using Anlab.Core.Models.AggieEnterpriseModels;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anlab.Core.Services
{
    public interface IAggieEnterpriseService
    {
        Task<AccountValidationModel> IsAccountValid(string financialSegmentString, bool validateCVRs = true);

        Task<FinancialOfficerDetails> GetFinancialOfficer(string financialSegmentString);
     
        Task<string> ConvertKfsAccount(string account);
    }

    public class AggieEnterpriseService : IAggieEnterpriseService
    {
        private IAggieEnterpriseClient _aggieClient;

        public AggieEnterpriseSettings AeSettings { get; set; }

        public AggieEnterpriseService(IOptions<AggieEnterpriseSettings> aggieEnterpriseSettings)
        {
            AeSettings = aggieEnterpriseSettings.Value;

            try
            {
                //_aggieClient = GraphQlClient.Get(AeSettings.GraphQlUrl, AeSettings.Token);
                _aggieClient = GraphQlClient.Get(AeSettings.GraphQlUrl, AeSettings.TokenEndpoint, AeSettings.ConsumerKey, AeSettings.ConsumerSecret, $"{AeSettings.ScopeApp}-{AeSettings.ScopeEnv}");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error creating Aggie Enterprise Client");
                Log.Information("Aggie Enterprise Scope {scope}", $"{AeSettings.ScopeApp}-{AeSettings.ScopeEnv}");
                _aggieClient = null;
            }
        }

        public async Task<AccountValidationModel> IsAccountValid(string financialSegmentString, bool validateCVRs = true)
        {
            var rtValue = new AccountValidationModel();
            rtValue.IsValid = false;
            rtValue.FinancialSegmentString = financialSegmentString;

            var segmentStringType = FinancialChartValidation.GetFinancialChartStringType(financialSegmentString);
            rtValue.CoaChartType = segmentStringType;

            if (segmentStringType == FinancialChartStringType.Gl)
            {
                var result = await _aggieClient.GlValidateChartstring.ExecuteAsync(financialSegmentString, validateCVRs);

                var data = result.ReadData();

                rtValue.IsValid = data.GlValidateChartstring.ValidationResponse.Valid;

                if (!rtValue.IsValid)
                {
                    foreach (var err in data.GlValidateChartstring.ValidationResponse.ErrorMessages)
                    {
                        rtValue.Messages.Add(err);
                    }
                }
                rtValue.Details.Add(new KeyValuePair<string, string>("Entity", $"{data.GlValidateChartstring.SegmentNames.EntityName} ({data.GlValidateChartstring.Segments.Entity})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Fund", $"{data.GlValidateChartstring.SegmentNames.FundName} ({data.GlValidateChartstring.Segments.Fund})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Department", $"{data.GlValidateChartstring.SegmentNames.DepartmentName} ({data.GlValidateChartstring.Segments.Department})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Account", $"{data.GlValidateChartstring.SegmentNames.AccountName} ({data.GlValidateChartstring.Segments.Account})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Purpose", $"{data.GlValidateChartstring.SegmentNames.PurposeName} ({data.GlValidateChartstring.Segments.Purpose})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Project", $"{data.GlValidateChartstring.SegmentNames.ProjectName} ({data.GlValidateChartstring.Segments.Project})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Program", $"{data.GlValidateChartstring.SegmentNames.ProgramName} ({data.GlValidateChartstring.Segments.Program})"));
                rtValue.Details.Add(new KeyValuePair<string, string>("Activity", $"{data.GlValidateChartstring.SegmentNames.ActivityName} ({data.GlValidateChartstring.Segments.Activity})"));

                if (data.GlValidateChartstring.Warnings != null)
                {
                    foreach (var warn in data.GlValidateChartstring.Warnings)
                    {
                        rtValue.Warnings.Add(new KeyValuePair<string, string>(warn.SegmentName, warn.Warning));
                    }
                }

                rtValue.GlSegments = FinancialChartValidation.GetGlSegments(financialSegmentString);

                rtValue.Description = $"{data.GlValidateChartstring.SegmentNames.DepartmentName} - {data.GlValidateChartstring.SegmentNames.FundName}";
                
                if(rtValue.GlSegments.Account != AeSettings.NaturalAccount)
                {
                    rtValue.Messages.Add($"Natural Account must be {AeSettings.NaturalAccount}");
                    rtValue.IsValid = false;
                }

                return rtValue;
            }

            if (segmentStringType == FinancialChartStringType.Ppm)
            {
                var result = await _aggieClient.PpmSegmentStringValidate.ExecuteAsync(financialSegmentString);

                var data = result.ReadData();

                rtValue.IsValid = data.PpmSegmentStringValidate.ValidationResponse.Valid;
                if (!rtValue.IsValid)
                {
                    foreach (var err in data.PpmSegmentStringValidate.ValidationResponse.ErrorMessages)
                    {
                        rtValue.Messages.Add(err);
                    }
                }

                rtValue.Details.Add(new KeyValuePair<string, string>("Project", data.PpmSegmentStringValidate.Segments.Project));
                rtValue.Details.Add(new KeyValuePair<string, string>("Task", data.PpmSegmentStringValidate.Segments.Task));
                rtValue.Details.Add(new KeyValuePair<string, string>("Organization", data.PpmSegmentStringValidate.Segments.Organization));
                rtValue.Details.Add(new KeyValuePair<string, string>("Expenditure Type", data.PpmSegmentStringValidate.Segments.ExpenditureType));
                rtValue.Details.Add(new KeyValuePair<string, string>("Award", data.PpmSegmentStringValidate.Segments.Award));
                rtValue.Details.Add(new KeyValuePair<string, string>("Funding Source", data.PpmSegmentStringValidate.Segments.FundingSource));

                if (data.PpmSegmentStringValidate.Warnings != null)
                {
                    foreach (var warn in data.PpmSegmentStringValidate.Warnings)
                    {
                        rtValue.Warnings.Add(new KeyValuePair<string, string>(warn.SegmentName, warn.Warning));
                    }
                }

                rtValue.PpmSegments = FinancialChartValidation.GetPpmSegments(financialSegmentString);


                await GetPpmAccountManager(rtValue);

                if(rtValue.PpmSegments.ExpenditureType != AeSettings.NaturalAccount)
                {
                    rtValue.Messages.Add($"Expenditure Type must be {AeSettings.NaturalAccount}");
                    rtValue.IsValid = false;
                }

                return rtValue;
            }

            rtValue.IsValid = false; //Just in case.
            rtValue.Messages.Add("Invalid Aggie Enterprise COA format");

            return rtValue;
        }

        public async Task<FinancialOfficerDetails> GetFinancialOfficer(string financialSegmentString)
        {
            //TODO: If AE can supply the query....
            var rtValue = new FinancialOfficerDetails();
            rtValue.FinancialOfficerId = null;

            return rtValue;
        }

        public async Task<string> ConvertKfsAccount(string account)
        {
            var parts = account.Split('-');

            if (parts.Length < 2)
            {
                return null;
            }

            var chart = parts[0].Trim();
            var accountPart = parts[1].Trim();
            var subAcct = parts.Length > 2 ? parts[2].Trim() : null;

            var result = await _aggieClient.KfsConvertAccount.ExecuteAsync(chart, accountPart, subAcct);
            var data = result.ReadData();
            if (data.KfsConvertAccount.GlSegments != null)
            {
                var tempGlSegments = new GlSegments(data.KfsConvertAccount.GlSegments);
                tempGlSegments.Account = AeSettings.NaturalAccount;
                return tempGlSegments.ToSegmentString();
            }

            if (data.KfsConvertAccount.PpmSegments != null)
            {
                var tempPpmSegments = new PpmSegments(data.KfsConvertAccount.PpmSegments);
                tempPpmSegments.ExpenditureType = AeSettings.NaturalAccount;
                return tempPpmSegments.ToSegmentString();
            }
            else
            {
                return null;
            }
        }

        private async Task GetPpmAccountManager(AccountValidationModel rtValue)
        {
            var result = await _aggieClient.PpmProjectManager.ExecuteAsync(rtValue.PpmSegments.Project);

            var data = result.ReadData();

            if (data.PpmProjectByNumber?.ProjectNumber == rtValue.PpmSegments.Project)
            {
                rtValue.AccountManager = data.PpmProjectByNumber.PrimaryProjectManagerName;
                rtValue.AccountManagerEmail = data.PpmProjectByNumber.PrimaryProjectManagerEmail;
                rtValue.Description = data.PpmProjectByNumber.Name;
            }
            return;
        }
    }
}
