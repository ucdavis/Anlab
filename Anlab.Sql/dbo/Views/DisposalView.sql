CREATE VIEW dbo.DisposalView
AS
SELECT        dbo.Orders.Id, dbo.AspNetUsers.Email, dbo.Orders.AdditionalEmails, dbo.Orders.ClientId, dbo.Orders.Status, dbo.Orders.DateFinalized, dbo.Orders.RequestNum, JSON_VALUE(dbo.Orders.JsonDetails, 
                         '$.LabworksSampleDisposition') AS LabworksSampleDisposition, JSON_VALUE(dbo.Orders.JsonDetails, '$.SampleDisposition') AS SampleDisposition
FROM            dbo.Orders INNER JOIN
                         dbo.AspNetUsers ON dbo.Orders.CreatorId = dbo.AspNetUsers.Id
WHERE        (dbo.Orders.IsDeleted = 0) AND (NOT (dbo.Orders.DateFinalized IS NULL)) AND (dbo.Orders.Status = N'Finalized' OR
                         dbo.Orders.Status = N'Complete')
GO


