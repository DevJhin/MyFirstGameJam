using UnityEngine;

/// <summary>
/// ������ �߻� �޽����� �����ϴ� Ŭ����.
/// �����ڿ� ���ط�, �ǰ� ������ �ԷµǾ� �ǰ� ��󿡰� ���޵˴ϴ�.
/// </summary>
public class DamageMessage : IEvent
{
    public DamageMessage() { }
    public DamageMessage(FieldObject attacker, float damage, Vector2 hitPoint)
	{
        this.attacker = attacker;
        this.damage = damage;
        this.hitPoint = hitPoint;
	}

    public FieldObject attacker;
    public Vector2 hitPoint;
    public float damage;
}
