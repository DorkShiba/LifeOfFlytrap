using UnityEngine;

// 기존 파리지옥 트랩 스크립트가 이 인터페이스를 구현하도록 연결하세요.
public interface ITrap
{
    Vector3 Position { get; }
    Vector2 ColliderSize { get; }
    float LandingRadius { get; }
    bool IsAvailable { get; }

}

// 에너지로 강화하는 "벌레가 잎에 더 잘 모이게" 업그레이드를 여기 연결하세요.
public interface ITrapAttractionProvider
{
    float GetMultiplier();
}