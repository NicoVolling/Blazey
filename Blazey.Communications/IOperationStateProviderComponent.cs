using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazey.Communications;

public interface IOperationStateProviderComponent
{
    public OperationStateProvider OperationState { get; set; }
}