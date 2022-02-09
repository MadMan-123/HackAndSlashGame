using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] CannonBall Round;
    [SerializeField] float fMaxDuration = 1.5f;
    [SerializeField] float fTimeStepInterval = 0.1f;
    [SerializeField] float fForce = 10.0f;
    [SerializeField] float fMass = 1.0f;

    private Transform Barrel;
    private Transform CannonBallSpawnPoint;
    private LineRenderer LineRenderer;
    private Transform Rotater;

    void Start()
    {
        Barrel = transform.GetChild(0).GetChild(0);
        CannonBallSpawnPoint = GetComponentInChildren<SpawnPos>().transform;
        LineRenderer = GetComponent<LineRenderer>();
        Rotater = transform.GetChild(0);
    }

    void Update()
    {
        UpdateForce();
        MoveCannon();
        DrawArc();
    }

    private List<Vector3> SimulateArc()
    {
        List<Vector3> LineRenderPoints = new List<Vector3>();
        int iMaxSteps = (int)(fMaxDuration / fTimeStepInterval);
        Vector3 vDir = CannonBallSpawnPoint.transform.forward;
        Vector3 vPos = CannonBallSpawnPoint.position;

        float fVel = (fForce / fMass) * Time.fixedDeltaTime;


        for (int i = 0; i < iMaxSteps; i++)
        {

            Vector3 CalcPos = vPos + vDir * fVel * i * fTimeStepInterval;
            CalcPos.y = Physics.gravity.y / 2.0f * Mathf.Pow(i * fTimeStepInterval, 2);

            LineRenderPoints.Add(CalcPos);

            if (Physics.CheckSphere(CalcPos, 0.1f))
            {
                break;
            }
        }



        return LineRenderPoints;
    }

    void DrawArc()
    {
        List<Vector3> LineRenderPoints = new List<Vector3>();
        LineRenderPoints = SimulateArc();

        LineRenderer.positionCount = LineRenderPoints.Count;
        for (int i = 0; i < LineRenderPoints.Count; i++)
        {
            LineRenderer.SetPosition(i, LineRenderPoints[i]);
        }

        LineRenderPoints.Clear();
    }

    void MoveCannon()
    {
        //if a is pressed rotate left and if d is pressed rotate right
        if (Input.GetKey(KeyCode.A))
        {
            Rotater.Rotate(Vector3.up, 1.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Rotater.Rotate(Vector3.up, -1.0f);
        }
    }

    void UpdateForce()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            fForce += 1.0f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            fForce -= 1.0f;
        }
    }

}
