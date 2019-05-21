CREATE VIEW dbo.DisposalView
AS
SELECT        dbo.Orders.Id, dbo.AspNetUsers.Email, dbo.Orders.AdditionalEmails, dbo.Orders.ClientId, dbo.Orders.Status, dbo.Orders.DateFinalized, dbo.Orders.RequestNum, JSON_VALUE(dbo.Orders.JsonDetails, 
                         '$.LabworksSampleDisposition') AS LabworksSampleDisposition, JSON_VALUE(dbo.Orders.JsonDetails, '$.SampleDisposition') AS SampleDisposition, 
                         COUNT(CASE WHEN dbo.MailMessages.Subject LIKE 'Work Request Disposal Warning%' THEN 1 ELSE NULL END) AS EmailCount
FROM            dbo.Orders INNER JOIN
                         dbo.AspNetUsers ON dbo.Orders.CreatorId = dbo.AspNetUsers.Id LEFT OUTER JOIN
                         dbo.MailMessages ON dbo.Orders.Id = dbo.MailMessages.OrderId
WHERE        (dbo.Orders.IsDeleted = 0) AND (NOT (dbo.Orders.DateFinalized IS NULL)) AND (dbo.Orders.Status = N'Finalized' OR
                         dbo.Orders.Status = N'Complete')
GROUP BY dbo.Orders.Id, dbo.AspNetUsers.Email, dbo.Orders.AdditionalEmails, dbo.Orders.ClientId, dbo.Orders.Status, dbo.Orders.DateFinalized, dbo.Orders.RequestNum, JSON_VALUE(dbo.Orders.JsonDetails, 
                         '$.LabworksSampleDisposition'), JSON_VALUE(dbo.Orders.JsonDetails, '$.SampleDisposition')
