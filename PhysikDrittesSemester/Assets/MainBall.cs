using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainBall : MonoBehaviour
{
    [SerializeField] private float Reibungswert;
    [SerializeField] private float GewichtDerKugel;
    [SerializeField] private bool IstNichtHauptKugel = false;

    // Liste aller Bälle
    public List<GameObject> balls;
    public float ballRadius = 0.0285f; // Radius der Bälle

    // Start is called before the first frame update
    void Start()
    {
        // Initalisierung der Liste, Bälle GameObjects hinzufügen
        
        balls = new List<GameObject> {
            GameObject.Find("weisseKugel"),
            GameObject.Find("ball1"),
            GameObject.Find("ball2"),
            GameObject.Find("ball3"),
            GameObject.Find("ball4"),
            GameObject.Find("ball5"),
            GameObject.Find("ball6"),
            GameObject.Find("ball7"),
            GameObject.Find("ball8"),
            GameObject.Find("ball9"),
            GameObject.Find("ball10"),
            GameObject.Find("ball11"),
            GameObject.Find("ball12"),
            GameObject.Find("ball13"),
            GameObject.Find("ball14"),
            GameObject.Find("ball15")
        };
    }

    // Update is called once per frame
    void Update()
    {
        CheckCollisions();
    }

    void CheckCollisions()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            for (int j = i + 1; j < balls.Count; j++)
            {
                if (AreBallsColliding(balls[i].transform.position, ballRadius, balls[j].transform.position, ballRadius))
                {
                    ChangeColor(balls[i]);
                    ChangeColor(balls[j]);
                    Debug.Log(balls[i].name + " and " + balls[j].name + " are colliding.");
                }
            }
        }
    }

    bool AreBallsColliding(Vector3 position1, float radius1, Vector3 position2, float radius2)
    {
        return Vector3.Distance(position1, position2) <= (radius1 + radius2);
    }

    void ChangeColor(GameObject ball)
    {
        SpriteRenderer renderer = ball.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = Color.blue; // Farbe ändern bei Kollision
        }
    }
}
