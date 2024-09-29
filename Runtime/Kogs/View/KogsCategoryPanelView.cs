using TMPro;
using UnityEngine;

namespace Kukuru3.Kogs.View {
    class KogsCategoryPanelView : MonoBehaviour {
        [field: SerializeField] internal RectTransform InsertionRoot { get; private set; }
        [field: SerializeField] TextMeshProUGUI title;

        internal string CategoryID { get; private set; }
        internal void Bind(string categoryID) {
            CategoryID = categoryID ?? "";
            title.text = string.IsNullOrEmpty(categoryID) ? "Debug info" : categoryID;
            gameObject.name = $"Panel : {categoryID}";
        }

        internal void SetState(bool active) => gameObject.SetActive(active);
    }
}