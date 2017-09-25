﻿
using System;

namespace Anlab.Jobs.MoneyMovement
{
    public class SlothResponseModel
    {
        public Guid Id { get; set; }
        public string KfsTrackingNumber { get; set; }
        public string Status { get; set; }

        public bool Success { get; set; } = true;
    }
}
