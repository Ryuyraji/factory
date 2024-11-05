using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorSimple : MonoBehaviour
{
    public float speed; // 컨베이어 벨트의 이동 속도(m/s)

    // 변경점: 선택한 방향과 실제로 오브젝트가 이동하는 방향이 반대이므로, 올바른 방향으로 이름을 맞춤
    public enum VectorDirection
    {
        forward, // "forward"는 실제로 뒤로 이동(back)을 의미함
        back,    // "back"은 실제로 앞으로 이동(forward)을 의미함
        right,   // "right"는 실제로 왼쪽(left)으로 이동을 의미함
        left     // "left"는 실제로 오른쪽(right)으로 이동을 의미함
    }

    public VectorDirection ChosenVec; // 사용자가 선택한 이동 방향을 저장하는 변수
    public bool Reverse = false; // 컨베이어 벨트의 이동을 역방향으로 할지 여부
    Rigidbody MyrbBody; // 이 오브젝트의 Rigidbody 컴포넌트를 저장하는 변수
    Material mymaterial; // 이 오브젝트의 Material 컴포넌트를 저장하는 변수

    // Start는 첫 프레임에서 스크립트가 실행될 때 호출됩니다.
    void Start()
    {
        this.MyrbBody = this.gameObject.GetComponent<Rigidbody>(); // 이 오브젝트의 Rigidbody를 가져와서 저장
        this.mymaterial = this.gameObject.GetComponent<Renderer>().material; // 이 오브젝트의 Material을 가져와서 저장
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        ScrollUV(); // 매 프레임마다 Material의 텍스처를 이동시키는 함수 호출
    }

    // FixedUpdate는 고정된 시간 간격마다 호출되며, 물리 계산에 사용됩니다.
    void FixedUpdate()
    {
        // 선택된 방향에 따라 오브젝트의 위치를 업데이트(앞,뒤,좌,우 움직이게 만들기)
        switch (ChosenVec)
        {
            case VectorDirection.forward: // 사용자가 "forward"를 선택한 경우 (실제 이동은 뒤로)
                Vector3 posB = MyrbBody.position; // 현재 위치를 저장
                MyrbBody.position += Vector3.back * speed * Time.fixedDeltaTime; // 뒤로 이동 (back 방향)
                MyrbBody.MovePosition(posB); // 이전 위치로 MovePosition을 통해 이동
                break;

            case VectorDirection.back: // 사용자가 "back"을 선택한 경우 (실제 이동은 앞으로)
                Vector3 posU = MyrbBody.position; // 현재 위치를 저장
                MyrbBody.position += Vector3.forward * speed * Time.fixedDeltaTime; // 앞으로 이동 (forward 방향)
                MyrbBody.MovePosition(posU); // 이전 위치로 MovePosition을 통해 이동
                break;

            case VectorDirection.right: // 사용자가 "right"를 선택한 경우 (실제 이동은 왼쪽으로)
                Vector3 posL = MyrbBody.position; // 현재 위치를 저장
                MyrbBody.position += Vector3.left * speed * Time.fixedDeltaTime; // 왼쪽으로 이동 (left 방향)
                MyrbBody.MovePosition(posL); // 이전 위치로 MovePosition을 통해 이동
                break;

            case VectorDirection.left: // 사용자가 "left"를 선택한 경우 (실제 이동은 오른쪽으로)
                Vector3 posR = MyrbBody.position; // 현재 위치를 저장
                MyrbBody.position += Vector3.right * speed * Time.fixedDeltaTime; // 오른쪽으로 이동 (right 방향)
                MyrbBody.MovePosition(posR); // 이전 위치로 MovePosition을 통해 이동
                break;
        }
    }

    /// <summary>
    /// Material도 이동시킴 (UV 좌표 변경)
    /// 
    /// offset은 원래 마테리얼의 타일링을 이동시키는 방향을 3으로 확장시키고 있음.
    /// 물리적으로 이동시키려면 Vector3.back(즉 "1") * speed(여기서는 "2") * time~~~ (두 번 적용되므로 무시)
    /// 그래서 1 * speed(2) = 2가 되므로, offset에도 동일한 값이 오도록 3배를 한 것을 3으로 나눔(/3 = *0.33...)
    /// 즉 텍스처의 y 스케일이 늘어난 만큼 반비례하여 offset을 적용함.
    /// </summary>
    void ScrollUV()
    {
        if (!Reverse) // 역방향이 아닌 경우
        {
            var material = this.mymaterial; // 오브젝트의 Material 가져오기
            Vector2 offset = material.mainTextureOffset; // 현재 텍스처의 오프셋을 가져옴
            offset += Vector2.up * speed * Time.deltaTime / material.mainTextureScale.y; // y 축을 기준으로 텍스처 오프셋 이동
            material.mainTextureOffset = offset; // 이동된 오프셋을 적용
        }

        // 변경점: 반대 방향으로 움직일 경우 텍스처 오프셋도 반대로 이동
        if (Reverse) // 역방향으로 움직이는 경우
        {
            var material = this.mymaterial; // 오브젝트의 Material 가져오기

            Vector2 TextureScale = this.mymaterial.mainTextureScale; // 현재 텍스처 스케일 가져오기
            TextureScale = new Vector2(1, -3f); // 텍스처의 y 스케일을 음수로 설정하여 뒤집음
            material.mainTextureScale = TextureScale; // 변경된 텍스처 스케일 적용

            Vector2 offset = material.mainTextureOffset; // 현재 텍스처의 오프셋을 가져옴
            offset += Vector2.down * speed * Time.deltaTime / material.mainTextureScale.y; // y 축을 기준으로 텍스처 오프셋을 아래로 이동
            material.mainTextureOffset = offset; // 이동된 오프셋을 적용
        }
    }
}
