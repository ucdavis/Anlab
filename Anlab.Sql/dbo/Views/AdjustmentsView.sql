CREATE VIEW dbo.AdjustmentsView
AS
SELECT Id,
RequestNum,
ClientId,
PaymentType,
Created,
Updated,
Status,
Paid,
DateFinalized,
CAST(JSON_VALUE(JsonDetails, '$.GrandTotal') AS DECIMAL(18, 2)) AS GrandTotal,
CAST(JSON_VALUE(JsonDetails, '$.AdjustmentAmount') AS DECIMAL(18, 2)) AS AdjustmentAmount, JSON_VALUE(JsonDetails, 
           '$.AdjustmentComments') AS Reason
FROM   dbo.Orders
WHERE (IsDeleted = 0) AND (CAST(JSON_VALUE(JsonDetails, '$.AdjustmentAmount') AS DECIMAL) <> 0.0)
GO
