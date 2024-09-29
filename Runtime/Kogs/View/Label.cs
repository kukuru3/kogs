
using UnityEngine;

namespace Kukuru3.Kogs.View {
    class Label : CogsDiagnosticView<string, LabelConfiguration> {
        [field: SerializeField] internal TMPro.TextMeshProUGUI Caption { get; private set; }

        internal Color baseLabelColor;

        private void Start() => baseLabelColor = Caption.fontSharedMaterial.GetColor(TMPro.ShaderUtilities.ID_FaceColor);

        internal override void Maintain(ViewData data) {
            _refresh = data.life < Config.linger;
            // var flash = 1f;
            if (_refresh) {
                Caption.alpha = 1f;
               //  flash = 1f;
            }
            // var c = Color.Lerp(baseLabelColor, new Color(4, 4f, 4f), flash);
            // Caption.fontMaterial.SetColor(TMPro.ShaderUtilities.ID_FaceColor, c);
            Caption.alpha = data.alpha;
            Caption.text = Value;

        }

        bool _refresh;

        internal override bool RefreshObsoletion => _refresh;
    }
}