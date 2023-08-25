#pragma warning disable 0649
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class AmmoPanel : MonoBehaviour
    {
        [SerializeField] private Image _cooldownImage;
        [SerializeField] private Text _ammoText;

        public void SetCooldown(float amount)
        {
            _cooldownImage.fillAmount = amount;
        }

        public void SetAmmo(int ammo)
        {
            _ammoText.text = ammo.ToString();
        }
    }
}
