using System;
using System.Collections.Generic;
using System.Text;

namespace Anlab.Core.Models
{
    public interface IDatedEntity
    {
        DateTime Created { get; set; }
        DateTime Updated { get; set; }
    }
}
