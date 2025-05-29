# SampleCode
Sample Code For Unity

코드 스타일 표현을 위한
Basic Framework. 


Framework Scene에서 실행.

화면 설명
1. 왼쪽 패널 UI Load
2. 왼쪽 패널 UI Delete
3. Scene Change.
4. Localization (KR, JP, EN)
5. 캐릭터소환 ( 소환 시 패널 등장 )
6. Spine Animation

![image](https://github.com/user-attachments/assets/2b1156c4-f9ba-4420-a61b-87f570af1722)


구조 UML
![image](https://github.com/user-attachments/assets/d5f248fa-ec0f-4e11-990f-d636b1ef3738)


기능 및 툴
1. Addressable을 통한 리소스 관리
2. Excel Export 툴을 통한 테이블 데이터 자동화.
   => 엑셀 테이블에서 CSV추출 후 Localization Table 자동화
   => 다른 엑셀테이블 추출 시 Json파일 및 Class 생성
   => _container를 통한 데이터 관리
   ![image](https://github.com/user-attachments/assets/377389e4-2bf8-4745-affa-655d586f3b64)

   ( Tools/Export to C# Class Exporter )

4. 캐릭터 MVC 적용.
=> ![image](https://github.com/user-attachments/assets/c220f278-1549-4606-80d5-2f1c7e7f308b)
5. UI Load, Character Load 등 어디에서 사용이 가능하도록 기능 분리 위주 구현.

