using System;

/// <summary>
/// MessageSystem에 등록해서 이벤트를 받는 역할을 수행
/// </summary>
public interface IEventListener
{
    /// <summary>
    /// 이벤트를 수신할 때 호출되는 함수입니다.
    /// </summary>
    /// <returns>false면 이벤트 받고 반응이 없게 짤 것</returns>
    bool OnEvent (IEvent e);
}

/// <summary>
/// 이벤트 인터페이스
/// </summary>
public interface IEvent
{
    
}
