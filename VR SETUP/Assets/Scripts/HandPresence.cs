using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class HandPresence : MonoBehaviour
{
    public bool showController = false;
    public InputDeviceCharacteristics controllerCharacteristics;
    public List<GameObject> controllerPrefabs;
    public GameObject handModelPrefab;

    private InputDevice targetDevice;
    private GameObject spawnedController;
    private GameObject spawnedHandModel;
    private Animator handAnimator;

    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();
    }

    void TryInitialize(){
        //Buat list kosong
        List<InputDevice> devices = new List<InputDevice>();
        //cara ngisi listnya dengan device yang terhubung
        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

        foreach (var item in devices)
        {
            //Untuk ngecek di log apakah devicenya dah bener + nama dan karakteristiknya
            Debug.Log(item.name + item.characteristics);
        }

        if(devices.Count > 0) {
            //buat ngambil device dari list
            targetDevice = devices[0];
            /*set prefab yang mau di render agar sesuai dengan nama devicenya, nah disini prefab yang kita siapin namanya
              harus bener bener sama dengan device yang ke detect*/
            GameObject  prefab = controllerPrefabs.Find(controller => controller.name == targetDevice.name);
            //Dia akan munculin prefabnya dengan transform
            if(prefab) {
                spawnedController = Instantiate(prefab, transform);
            }
            //kalo namanya beda, kita pake prefab 0 / default controller
            else {
                Debug.LogError("Did not find corresponding controller model");
                spawnedController = Instantiate(controllerPrefabs[0],transform);
            }
            //transform prefab
            spawnedHandModel = Instantiate(handModelPrefab, transform);
            //ambil animator yang ada di prefabnya
            handAnimator = spawnedHandModel.GetComponent<Animator>();
       }
    }
    //Buat njalanin animasi prefabnya. jadi di set di unity dulu floatnya sama animasinya
    void UpdateHandAnimation(){
        if(targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)){
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else {
            handAnimator.SetFloat("Trigger", 0);
        }

        if(targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue)){
            handAnimator.SetFloat("Grip", gripValue);
        }
        else {
            handAnimator.SetFloat("Grip", 0);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!targetDevice.isValid){
            TryInitialize();
        }
        else{
            if(showController){
            spawnedHandModel.SetActive(false);
            spawnedController.SetActive(true);
            }
            else {
                spawnedHandModel.SetActive(true);
                spawnedController.SetActive(false);
                UpdateHandAnimation();
            }
        }
    }
}
