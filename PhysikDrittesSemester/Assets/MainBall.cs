using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Hauptklasse für die Ball-Interaktionen
public class MainBall : MonoBehaviour
{
    public GameObject[] balls; // Array Balls
    public Wall[] walls; // Array Walls
    public float ballRadius = 0.0285f; // Radius der Bälle
    public Color collisionColor = Color.blue; // Farbe bei Kollision
    public float collisionDuration = 0.1f; // Dauer Kollisionsfarbänderung
    public float repulsionForce = 0.5f; // Abstoßungskraft Kollisionen
    public float initialSpeed = 2f; // Startgeschwindigkeit Bälle
    public float friction = 0.1f; // Reibung

    private Color[] originalColors; // Ursprünglichsfarbe Bälle
    private Vector2[] ballVelocities; // Bällegeschwindigkeit

 
    void Start()
    {
        FindBalls(); // Bälle suchen und initialisieren
        FindWalls(); // Wände suchen

        for (int i = 1; i < balls.Length; i++)
        {
            ballVelocities[i] = Vector2.zero; // Geschwindigkeiten aller Bälle, außer dem weißen Ball auf Null setzen
        }
    }

 
    void Update()
    {
        MoveBalls(); // Bewegt Bälle
        CheckCollisions(); // Überprüft Kollisionen

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PushWhiteBallTowardsMouse(); // Weiße Kugel wird zur Mausposition geschoben abhängig von der Bildschirmmitte
        }
    }

    // alle Bälle suchen und Originalfarbe speichern, initialisieren der Geschwindigkeit
    void FindBalls()
    {
        balls = GameObject.FindGameObjectsWithTag("Ball");

        originalColors = new Color[balls.Length];
        ballVelocities = new Vector2[balls.Length];

        for (int i = 0; i < balls.Length; i++)
        {
            SpriteRenderer renderer = balls[i].GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                originalColors[i] = renderer.color; // Speichert Originalfarbe
            }

            ballVelocities[i] = Vector2.zero; // Initialisiert Geschwindigkeit = 0
        }
    }

    // Alle Wände suchen
    void FindWalls()
    {
        walls = FindObjectsOfType<Wall>(); // Alle "Walls finden
    }

    // Alle bälle bewegen nach geschwindigkeit und Zeit
    void MoveBalls()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            balls[i].transform.position += (Vector3)ballVelocities[i] * Time.deltaTime; // anpassen der Position

            ballVelocities[i] *= (1 - friction * Time.deltaTime); //Geschwindigkeit nach reibung anpassen                                                             
        }
    }

    //Prüft Kollision von Bällen mit Wänden und anderen Kugeln
    void CheckCollisions()
    {
        for (int i = 0; i < balls.Length; i++)
        {
            // Kollisionsbehandlung mit Wänden
            foreach (var wall in walls)
            {
                if (wall.IsCollidingWithBall(balls[i], ballRadius))
                {
                    Vector2 normal = wall.GetCollisionNormal(balls[i].transform.position, ballRadius);
                    Vector2 incomingVelocity = ballVelocities[i];
                    Vector2 reflectedVelocity = Vector2.Reflect(incomingVelocity, normal);
                    ballVelocities[i] = reflectedVelocity;

                    Vector2 newPosition = (Vector2)balls[i].transform.position + reflectedVelocity * Time.deltaTime;
                    balls[i].transform.position = (Vector3)newPosition;

                    StartCoroutine(ResetColorAfterDelay(i)); // Farbe nach Verzögerung wieder zurücksetzen
                }
            }

            // Kollision mit anderen Bällen
            for (int j = 0; j < balls.Length; j++)
            {
                if (i != j && AreBallsColliding(balls[i], balls[j], ballRadius * 2))
                {
                    Vector2 direction = (balls[i].transform.position - balls[j].transform.position).normalized;
                    Vector2 repulsionVelocity = direction * repulsionForce;

                    //Abstoßkraft auf beide Bälle anwenden, bei Kollision
                   
                    ballVelocities[i] += repulsionVelocity * Time.deltaTime;
                    ballVelocities[j] -= repulsionVelocity * Time.deltaTime;

                    // ändert die Farbe der kollidierenden Bälle
                    ChangeColor(i);
                    ChangeColor(j);
                }
            }

            // Kollision mit der weissen Kugel
            if (i != 0 && AreBallsColliding(balls[i], balls[0], ballRadius * 2))
            {
                PushBallAwayFromWhiteBall(i);
                StartCoroutine(ResetColorAfterDelay(i)); // Farbe zurücksetzen nach Verzögerung
            }
        }
    }

    // Checkt ob zwei Bälle kollidieren in Bezug auf ihren Abstand zueinander
    bool AreBallsColliding(GameObject ball1, GameObject ball2, float collisionDistance)
    {
        float distance = Vector2.Distance(ball1.transform.position, ball2.transform.position);
        return distance <= collisionDistance;
    }

    // warte Zeit ab, dann Farbe des Balls wieder auf Originalfarbe setzen
    IEnumerator ResetColorAfterDelay(int index)
    {
        yield return new WaitForSeconds(collisionDuration);

        SpriteRenderer renderer = balls[index].GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = originalColors[index]; // setzt Originalfarbe zurück
        }
    }

    // weisse Kugel richtung Mausposition bewegen
    void PushWhiteBallTowardsMouse()
    {
        GameObject whiteBall = balls[0];

        // Mausposition in Weltkoordinaten umrechnen
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Sicherstellen, dass die Mausposition die selbe Z-Koordinate wie der weiße Ball hat

        // Berechnet die Richtung von der weissen Kugel zur Mausposition
        Vector2 direction = ((Vector2)mousePosition - (Vector2)whiteBall.transform.position).normalized;

        // Y-Komponente invertieren, um die korrekte Bewegungsrichtung zu gewährleisten
        direction.y *= -1;

        // Abstoßungskraft in berechnete Richtung nutzen
        ballVelocities[0] += direction * repulsionForce * Time.deltaTime;
    }

    // Schiebt jeweiligen Ball weg vom weissen Ball
    void PushBallAwayFromWhiteBall(int index)
    {
        GameObject whiteBall = balls[0];
        Vector2 direction = ((Vector2)balls[index].transform.position - (Vector2)whiteBall.transform.position).normalized;
        Vector2 repulsionVelocity = direction * repulsionForce * 2;

        float maxVelocityChange = 0.5f;
        Vector2 currentVelocity = ballVelocities[index];
        Vector2 newVelocity = currentVelocity + repulsionVelocity;
        if (newVelocity.magnitude > currentVelocity.magnitude + maxVelocityChange)
        {
            newVelocity = currentVelocity + (repulsionVelocity.normalized * maxVelocityChange);
        }

        ballVelocities[index] = newVelocity;
    }

    // Ändert Farbe des Balls bei Kollision
    void ChangeColor(int index)
    {
        SpriteRenderer renderer = balls[index].GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            renderer.color = collisionColor; // Ändert Farbe bei Kollisionm
        }
    }
}
