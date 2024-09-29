#if K3CORE
using K3.Modules;
#endif

using System.Diagnostics;
using System.Collections.Generic;

namespace Kukuru3.Kogs {
    internal class CogsSourceInferrer : KogsKomponent, IExecutesFrame, IExecutesTick {
        readonly Dictionary<int, int> tickHashCounter = new();
        readonly Dictionary<int, int> frameHashCounter = new();
        public void Frame() => frameHashCounter.Clear();

        public void Tick() => tickHashCounter.Clear();

        internal void PopulateWithCallsiteInfo(ref DispatchRequest invoker) {
            var stackFrame = new StackFrame(1);
            var hash = stackFrame.GetMethod().GetHashCode();
            invoker.callsiteHash = hash;
            invoker.callsiteTiming = KogsUtility.InferLoopTiming();
            invoker.callsiteCounter = GetCallsiteCounter(hash, invoker.callsiteTiming);
        }

        int GetCallsiteCounter(int hash, Timings timing) {
            var hashCounter = timing switch {
                Timings.Tick => tickHashCounter,
                _ => frameHashCounter
            };

            if (!hashCounter.TryGetValue(hash, out var ctr)) {
                ctr = 1;
            } else {
                ctr++;
            }
            hashCounter[hash] = ctr;
            return ctr;
        }
    }
}
