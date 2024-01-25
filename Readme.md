# dduR의 개인 new UnityInputSystem 사용하기 위한 리포지토리
input을 매번 만들기 귀찮아서 만든 리포지토리.

# 기능

Touch의 
    None,
    Began,
    Move,
    Held,
    Ended,
    Canceled
의 상태로 나뉘게 하였습니다.
다만 Move는 이제 Position 에서 적용할 예정

**Ended일때 RayCast를 쏘아서 Collider를 감지합니다.**
**Began, Hold, Ended때 UI를 모두 감지합니다.**
