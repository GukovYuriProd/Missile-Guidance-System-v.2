using System;
using System.Threading;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class CrosshairsController : MonoBehaviour
{
    public Camera normal_camera;
    public Camera thermal_camera;
    
    public GameObject normal_DinamicCircle;
    public GameObject normal_DinamicCrosshair;
    public GameObject normal_StaticChrest;
    public GameObject thermal_DinamicCircle;
    public GameObject thermal_DinamicCrosshair;
    public GameObject thermal_StaticChrest;
    
    public GameObject rocket_body;
    public Rigidbody rocket_rb;
    public GameObject drone_body;
    public Rigidbody drone_rb;
    
    public TextMeshProUGUI normal_UI_rocketX;
    public TextMeshProUGUI normal_UI_rocketY;
    public TextMeshProUGUI normal_UI_rocketZ;
    
    public TextMeshProUGUI normal_UI_targetX;
    public TextMeshProUGUI normal_UI_targetY;
    public TextMeshProUGUI normal_UI_targetZ;

    public TextMeshProUGUI normal_UI_rocketToTargetAngelX;
    public TextMeshProUGUI normal_UI_rocketToTargetAngelY;
    public TextMeshProUGUI normal_UI_rocketDistance;
    
    public TextMeshProUGUI normal_UI_rocketXRot;
    public TextMeshProUGUI normal_UI_rocketYRot;
    
    public TextMeshProUGUI normal_UI_rocketАccuracyNumber;
    public TextMeshProUGUI normal_UI_rocketStatus;
    
    public TextMeshProUGUI thermal_UI_rocketX;
    public TextMeshProUGUI thermal_UI_rocketY;
    public TextMeshProUGUI thermal_UI_rocketZ;
    
    public TextMeshProUGUI thermal_UI_targetX;
    public TextMeshProUGUI thermal_UI_targetY;
    public TextMeshProUGUI thermal_UI_targetZ;

    public TextMeshProUGUI thermal_UI_rocketToTargetAngelX;
    public TextMeshProUGUI thermal_UI_rocketToTargetAngelY;
    public TextMeshProUGUI thermal_UI_rocketDistance;
    
    public TextMeshProUGUI thermal_UI_rocketXRot;
    public TextMeshProUGUI thermal_UI_rocketYRot;
    
    public TextMeshProUGUI thermal_UI_rocketАccuracyNumber;
    public TextMeshProUGUI thermal_UI_rocketStatus;

    private float crosshairNormalDefaultX = -28.905f;
    private float crosshairNormalDefaultY = 5.085f;
    private float crosshairThermalDefaultX = -2.05f;
    private float crosshairThermalDefaultY = 5.057f;
    
    public float correctingSpeed = 0f;
    public float rocket_speed = 280f;
    private bool targetLocked = false;
    private bool targetFinded = false;
    private static bool rocketStarted = false;
    
    public float xVariable = 0.425f;
    public float yVariable = 0.49f;
    
    Vector3 _standardPrediction;
    private float _maxDistancePredict = 0f;

    public GameObject explosionPrefab;
    
    
    void FixedUpdate()
    {
        var rocketPosition = rocket_body.transform.position;
        normal_UI_rocketX.SetText(rocketPosition.x.ToString());
        normal_UI_rocketY.SetText(rocketPosition.y.ToString());
        normal_UI_rocketZ.SetText(rocketPosition.z.ToString());
        thermal_UI_rocketX.SetText(rocketPosition.x.ToString());
        thermal_UI_rocketY.SetText(rocketPosition.y.ToString());
        thermal_UI_rocketZ.SetText(rocketPosition.z.ToString());
        
        var targetPosition = drone_body.transform.position;
        normal_UI_targetX.SetText(targetPosition.x.ToString());
        normal_UI_targetY.SetText(targetPosition.y.ToString());
        normal_UI_targetZ.SetText(targetPosition.z.ToString());
        thermal_UI_targetX.SetText(targetPosition.x.ToString());
        thermal_UI_targetY.SetText(targetPosition.y.ToString());
        thermal_UI_targetZ.SetText(targetPosition.z.ToString());
        
        //Рассчет расстояния до цели по инерциальным координатам
        Vector3 rocketPositionForDistance = rocket_body.transform.position;
        Vector3 targetPositionForDistance = drone_body.transform.position;
        float distancebetweenTargetAndRocket = Vector3.Distance(rocketPositionForDistance, targetPositionForDistance);
        normal_UI_rocketDistance.SetText(Math.Round(distancebetweenTargetAndRocket, 0).ToString());
        thermal_UI_rocketDistance.SetText(Math.Round(distancebetweenTargetAndRocket, 0).ToString());
        
        //Вращение ракеты в инерциальных координатах
        normal_UI_rocketXRot.SetText(Math.Round(rocket_body.transform.rotation.x* Mathf.Rad2Deg * 2, 0).ToString());
        normal_UI_rocketYRot.SetText(Math.Round(rocket_body.transform.rotation.y* Mathf.Rad2Deg * 2, 0).ToString());
        thermal_UI_rocketXRot.SetText(Math.Round(rocket_body.transform.rotation.x* Mathf.Rad2Deg * 2, 0).ToString());
        thermal_UI_rocketYRot.SetText(Math.Round(rocket_body.transform.rotation.y* Mathf.Rad2Deg * 2, 0).ToString());
        
        
        
        //алгоритм рассчета точного инерциального смещения цели по системе ракеты
        //САМЫЙ ВАЖНЫЙ КУСОК, КАК ГОВОРИТСЯ ПОЕХАЛИИИИ
        if (rocketStarted)
        {
            thermal_camera.focalLength = 380f;
            rocket_rb.velocity = rocket_body.transform.forward * rocket_speed;
            float _minDistancePredict = -200f;
            float _maxTimePrediction = 8f;
            var leadTimePercentage = Mathf.InverseLerp(_minDistancePredict, _maxDistancePredict, Vector3.Distance(transform.position, drone_body.transform.position));
            var predictionTime = Mathf.Lerp(0, _maxTimePrediction, leadTimePercentage);
            _standardPrediction = drone_rb.position + drone_rb.velocity * predictionTime;
            _standardPrediction.x += 7f;
        }
        else
        {
            _standardPrediction = drone_rb.position;
        }
        
        rocket_body.transform.rotation = Quaternion.Slerp(rocket_body.transform.rotation,
            Quaternion.LookRotation(_standardPrediction - rocket_body.transform.position),
            Time.deltaTime * correctingSpeed);
        
        var shiftQuaternion = Quaternion.LookRotation(drone_body.transform.position - rocket_body.transform.position);
        Vector3 resultedClearShift = rocket_body.transform.rotation.eulerAngles - shiftQuaternion.eulerAngles;
        float clearVerticalShift = resultedClearShift.x;
        float clearHorisontalShift = -1*resultedClearShift.y;
        normal_UI_rocketToTargetAngelX.SetText(Math.Round(clearVerticalShift, 2).ToString());
        normal_UI_rocketToTargetAngelY.SetText(Math.Round(clearHorisontalShift, 2).ToString());
        thermal_UI_rocketToTargetAngelX.SetText(Math.Round(clearVerticalShift, 2).ToString());
        thermal_UI_rocketToTargetAngelY.SetText(Math.Round(clearHorisontalShift, 2).ToString());
        
        //Рассчет точности на данный момент
        float totalAccuracy = (float)Math.Sqrt(clearVerticalShift*clearVerticalShift + clearHorisontalShift*clearHorisontalShift);
        normal_UI_rocketАccuracyNumber.SetText(Math.Round(totalAccuracy, 2).ToString());
        thermal_UI_rocketАccuracyNumber.SetText(Math.Round(totalAccuracy, 2).ToString());
        if (totalAccuracy < 6)
        {
            normal_UI_rocketStatus.SetText("LOCK");
            thermal_UI_rocketStatus.SetText("LOCK");
        }
        else
        {
            normal_UI_rocketStatus.SetText("FIND");
            thermal_UI_rocketStatus.SetText("FIND");
        }
        
        //Определение положения крестика обнаружения на экранах наведения
        var thermalCrosshairPosition = new Vector3(crosshairThermalDefaultX, crosshairThermalDefaultY, -0.099f);
        thermal_DinamicCrosshair.transform.localPosition = thermalCrosshairPosition;
        var normalCrosshairPosition = new Vector3(crosshairNormalDefaultX, crosshairNormalDefaultY, -0.099f);
        normal_DinamicCrosshair.transform.localPosition = normalCrosshairPosition;

        if (targetFinded)
        {
            if (Vector3.Distance(transform.position, drone_body.transform.position) > 400f)
            {
                var thermalCrosshairPositionCorrected = thermalCrosshairPosition;
                thermalCrosshairPositionCorrected.x += clearHorisontalShift*xVariable * (thermal_camera.focalLength/50f);
                thermalCrosshairPositionCorrected.y += clearVerticalShift*yVariable * (thermal_camera.focalLength/50f);
                thermal_DinamicCrosshair.transform.localPosition = thermalCrosshairPositionCorrected;
                var normalCrosshairPositionCorrected = normalCrosshairPosition;
                normalCrosshairPositionCorrected.x += clearHorisontalShift*xVariable * (normal_camera.focalLength/50f);
                normalCrosshairPositionCorrected.y += clearVerticalShift*yVariable * (normal_camera.focalLength/50f);
                normal_DinamicCrosshair.transform.localPosition = normalCrosshairPositionCorrected;
            }
        if (Vector3.Distance(transform.position, drone_body.transform.position) < 5f) selfDestroy();
        }
        
        if (targetLocked)
        {
            if (targetFinded)
            {
                Random rnd = new Random();
                correctingSpeed = 1.6f + (float)rnd.Next(-10, 10) / 10;
            }
            else targetLocked = false;
        }
        
    }

    private void selfDestroy()
    {
        Debug.Log("Boom!");
        Instantiate(explosionPrefab, rocket_body.transform.position, rocket_body.transform.rotation);
        Destroy(drone_body);
        Destroy(rocket_body);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(rocket_body.transform.position, _standardPrediction);
    }

    public void updateNormalCameraValue()
    {
        if (normal_camera.focalLength < 1560)
        {
            normal_camera.focalLength += 300f;
        }
        else
        {
            normal_camera.focalLength = 50f;
        }
    }
    
    public void updateThermalCameraValue()
    {
        if (thermal_camera.focalLength < 1560)
        {
            thermal_camera.focalLength += 300f;
        }
        else
        {
            thermal_camera.focalLength = 50f;
        }
    }

    public void targetFind()
    {
        targetFinded = true;
    }

    public void targetLock()
    {
        targetLocked = true;
    }

    public void rocketStart()
    {
        _maxDistancePredict = Vector3.Distance(transform.position, drone_body.transform.position);
        rocketStarted = true;
    }

    public static bool getRocketStatus()
    {
        return rocketStarted;
    }
}