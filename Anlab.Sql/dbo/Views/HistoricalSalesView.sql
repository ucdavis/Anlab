CREATE VIEW dbo.HistoricalSalesView
AS
SELECT Id, DateFinalized, JSON_QUERY(JsonDetails, '$.SelectedTests') AS SelectedTests, CAST(JSON_VALUE(JsonDetails, '$.Payment.IsInternalClient') AS bit) AS IsInternal, CAST(ISNULL(JSON_VALUE(JsonDetails, '$.RushMultiplier'), 1.0) AS decimal(10, 2)) AS Rush, CAST(JSON_VALUE(JsonDetails, '$.InternalProcessingFee') 
           AS decimal(10, 2)) AS InternalProcessingFee, CAST(JSON_VALUE(JsonDetails, '$.ExternalProcessingFee') AS decimal(10, 2)) AS ExternalProcessingFee, CAST(JSON_VALUE(JsonDetails, '$.Quantity') AS int) AS Quantity
FROM   dbo.Orders
WHERE (DateFinalized IS NOT NULL)
GO
