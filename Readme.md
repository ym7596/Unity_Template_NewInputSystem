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
# 25-01-02

이제 에디터에서 특정 오브젝트를 클릭후 이동하게 하는 데모가 추가됩니다.
branch - dev/roomEditor에서 업데이트 되게 됩니다.

# 24-02-08

이제 카메라 컨트롤러가 생깁니다.

현재 카메라 컨트롤링 기능은 
**WASD QE에 대한 키맵핑의 이동 (앞뒤좌우상하)**

**QE (상하) 이동의 경우 World 좌표계로 이동합니다.**

**마우스**
    우클릭 + 이동 (회전)
    스크롤클릭 + 이동 (평면이동)
    스크롤휠 (줌인 아웃)
