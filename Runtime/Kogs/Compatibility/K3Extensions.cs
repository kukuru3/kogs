#if !K3CORE
using UnityEngine;

namespace Kukuru3.Kogs {
    static internal class K3Extensions {
        public static float Map(this float source, float sourceFrom, float sourceTo, float targetFrom, float targetTo, bool constrained = true) {
            var t = (source - sourceFrom) / (sourceTo - sourceFrom);
            if (constrained) t = Mathf.Clamp01(t);
            return targetFrom + t * (targetTo - targetFrom);
        }

        public static int Map(this int source, int sourceFrom, int sourceTo, int targetFrom, int targetTo, bool constrained = true) {
            var t = (float)(source - sourceFrom) / (sourceTo - sourceFrom);
            if (constrained) t = Mathf.Clamp01(t);
            return Mathf.RoundToInt(targetFrom + t * (targetTo - targetFrom));
        }
    }
}
#endif