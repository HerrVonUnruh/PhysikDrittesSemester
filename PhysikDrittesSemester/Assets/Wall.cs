using UnityEngine;

public class Wall : MonoBehaviour
{
    public Color collisionColor = Color.red; // Farbe Darstellung der Kollision

    public float wallLength = 1f; 
    public float wallHeight = 1f; 
    public Vector2 collisionOffset = Vector2.zero; // Killision Offset

    // zeichnen der Wand mit Gizmos, um die Kollision darzustellen
    private void OnDrawGizmos()
    {
        Gizmos.color = collisionColor; // Setzt die Farbe der Gizmos
        Vector2 size = new Vector2(wallLength, wallHeight); // Größe Gizmos festlegen
        Vector2 position = (Vector2)transform.position + collisionOffset; // Position Gizmos berechnen
        Gizmos.DrawWireCube(position, size);  //Zeichnet Außenseiten eines Qzuadrates an berechneter Position und Größe

    }

    // Überprüft, obKollision zwischen Wänden und einem Ball stattfindet
    public bool IsCollidingWithBall(GameObject ball, float ballRadius)
    {
        Vector2 ballPosition = ball.transform.position; // Position des Balls
        Vector2 wallPosition = (Vector2)transform.position + collisionOffset; // Berechne Wandposition
        Vector2 wallSize = new Vector2(wallLength, wallHeight); // Größe der Wand

        // Berechnet Grenzen der Wand inklusive Ballradius für bessere KollisionsErkennung
        float minX = wallPosition.x - wallSize.x / 2 - ballRadius;
        float maxX = wallPosition.x + wallSize.x / 2 + ballRadius;
        float minY = wallPosition.y - wallSize.y / 2 - ballRadius;
        float maxY = wallPosition.y + wallSize.y / 2 + ballRadius;

        // Überprüft, ob die Ballposition innerhalb der Kollisionsgrenze liegt
        return ballPosition.x >= minX && ballPosition.x <= maxX &&
               ballPosition.y >= minY && ballPosition.y <= maxY;
    }

    // Berechnet die Normale der Kollision basierendauf Position vom Ball relative zur Wand
    public Vector2 GetCollisionNormal(Vector2 ballPosition, float ballRadius)
    {
        Vector2 wallPosition = (Vector2)transform.position + collisionOffset; // Berechnete Wandposition
        Vector2 wallSize = new Vector2(wallLength, wallHeight); // Größe der Wand

        // Berechnet die minimalen und maximalen X- und Y-Werte, ohne den Ballradius
        float minX = wallPosition.x - wallSize.x / 2;
        float maxX = wallPosition.x + wallSize.x / 2;
        float minY = wallPosition.y - wallSize.y / 2;
        float maxY = wallPosition.y + wallSize.y / 2;

        Vector2 normal = Vector2.zero; // Initialisiert die Normale als Nullvektor

        // Bestimmt die X-Komponente der Normale, wenn der Ball nahe der horizontalen Grenzen ist
        if (ballPosition.x < minX + ballRadius || ballPosition.x > maxX - ballRadius)
        {
            normal.x = Mathf.Sign(ballPosition.x - wallPosition.x); // Richtung der Normalen basiert auf Ballposition
        }

        // Bestimmt die Y-Komponente der Normale, wenn der Ball nahe der vertikalen Grenze ist
        if (ballPosition.y < minY + ballRadius || ballPosition.y > maxY - ballRadius)
        {
            normal.y = Mathf.Sign(ballPosition.y - wallPosition.y); // Richtung der Normalen basierend auf Ballposition
        }

        return normal.normalized; // returned die normierte Normale

    }
}
