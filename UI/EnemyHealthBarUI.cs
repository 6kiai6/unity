using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBarUI : MonoBehaviour
{
    public Image healthSlider;
    private Transform cameraTransform;

    void Start(){
        cameraTransform = Camera.main.transform;
        healthSlider.fillAmount = 1;
    }

    void Update(){
        Vector3 dir = cameraTransform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-dir);
    }

    public void UpdateHealthBar(float healthRatio){
        healthSlider.fillAmount = healthRatio;
    }

}
