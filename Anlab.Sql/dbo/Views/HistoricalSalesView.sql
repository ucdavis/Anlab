CREATE VIEW dbo.HistoricalSalesView
AS
SELECT Id, DateFinalized, JSON_QUERY(JsonDetails, '$.SelectedTests') AS SelectedTests, JSON_VALUE(JsonDetails, '$.Payment.IsInternalClient') AS IsInternal, ISNULL(JSON_VALUE(JsonDetails, '$.RushMultiplier'), 1.0) AS Rush, JSON_VALUE(JsonDetails, '$.InternalProcessingFee') AS InternalProcessingFee, JSON_VALUE(JsonDetails, 
           '$.ExternalProcessingFee') AS ExternalProcessingFee, JSON_VALUE(JsonDetails, '$.Quantity') AS Quantity
FROM   dbo.Orders
WHERE (DateFinalized IS NOT NULL)
GO
