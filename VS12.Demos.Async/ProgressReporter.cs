using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AA.Demo.Model
{
   public class ProgressReporter<T> : IProgress<T>
    {
       private SynchronizationContext _context;
       private Action<T> _action;
       public ProgressReporter(Action<T> action)
       {
           _context = SynchronizationContext.Current;
           _action = action;
       }
        public void Report(T value)
        {
            _context.Post((c)=>_action(value),null);
        }


    }
}
