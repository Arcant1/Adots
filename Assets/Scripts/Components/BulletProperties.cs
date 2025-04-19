using Unity.Entities;

// Properties for bullets
public struct BulletProperties : IComponentData
{
    public float Speed;        // Speed of bullet movement
    public float Lifetime;     // How long the bullet exists before being destroyed
    public float CurrentTime;  // Current lifetime counter
} 