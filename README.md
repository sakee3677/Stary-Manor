# 專案名稱:Stary-Manor / 迷途莊園
![menu](https://github.com/user-attachments/assets/328fb70a-e99e-42b5-b236-34b3bd61fc09)

> 一款基於 Unity 的 Isometric (等距視角) 2D物理解謎遊戲
> **核心機制：** 靈魂附身系統 / 物理互動解謎

---

### 🕵️ 故事背景 (Story)
主角是一位私家偵探，受託進入一座神秘莊園進行調查，卻在途中遭到不明襲擊。
當主角再次醒來時，發現自己的身體發生了異變——**變成了一具幽靈**。
雖然失去了肉體，但也意外獲得了**附身 (Possession)**的能力。玩家必須利用這個能力，操控莊園內的物品，解開機關並查出真相。


### 📸 展示 (Showcase)

| 👻 附身機制 (Parasite) | 
|  ![Parasite](https://github.com/user-attachments/assets/394d0ab3-14a1-401b-a190-30091fc4712a) | 
 *描述：附身到史萊姆身上* 

---

###  技術實作細節 (Technical Details)

這是我用來練習 Unity 玩法架構的專案，主要技術亮點如下：

#### 核心程式系統 (Core Programming)
#### 核心角色控制器 (Player Controller Implementation)
腳本：`PlayerController.cs`
負責處理主角的移動、物理判定以及附身機制的狀態切換。

* **Isometric 輸入轉換 (Coordinate Conversion):**
    * 將玩家的二維輸入向量 (Vector2 Input) 轉換為 Isometric 45度視角的移動邏輯。
    * 實作了 4 方向鎖定 (4-way locking)，確保角色在等距視角下的移動符合視覺預期，避免斜向滑動的違和感。

* **附身狀態機 (Possession State Machine):**
    * 使用 `Enum` 定義附身階段 (`Prepare` -> `Parasiting` -> `None`)。
    * **平滑過渡 (Smooth Transition):** 在 `Prepare` 階段使用 `Vector3.MoveTowards` 實現靈魂被吸入物體的平滑位移，並在接觸瞬間切換物理碰撞層 (Collision Layer)。

* **動態材質交互 (Dynamic Material Interaction):**
    * 透過程式碼即時控制 Shader 參數 (`_GlowGlobal`, `_HandDrawnAmount`)。
    * 實作了線性插值 (Lerp) 算法，讓角色在附身/脫離時產生平滑的發光漸變與透明度變化，增強視覺回饋 (Juice)。

* **介面導向設計 (Interface Oriented):**
    * 透過 `IUsable` 與 `IControllable` 介面與場景物件互動，降低角色與具體物品邏輯的耦合度 (Coupling)。
* **非同步視覺處理 (Asynchronous VFX Handling):**
    * 使用 Coroutines (協程) 來處理附身過程中的時間序列動畫與材質變化，避免在 Update 循環中堆積過多狀態判斷，優化了程式碼的可讀性與效能。
 
> 🔗 **程式碼閱覽 (Code Highlight)：**
> * [PlayerController.cs](./Assets/Scripts/PlayerController.cs) -[Uploading playerController.cs…]()
 處理角色移動與附身狀態切換
> * [PossessableObject.cs](./Assets/Scripts/PossessableObject.cs) - 物件被附身後的行為邏輯


---

### 🚀 如何試玩 (How to Play)
1.  從這裡下載 Demo 版本：[]
2.  解壓縮後執行遊戲。
3.  **操作說明：**
    * **WASD**: 移動靈魂 / 移動被附身的物件
    * **E 鍵**: 對著物體按下，進行附身 / 解除附身
    * **Space**: (如果有跳躍或特殊能力可寫在這裡)

---

### 📝 未來優化方向 (Roadmap)
* [ ] 新增更多種類的附身物件（例如：可以發電的電器、可以飄浮的氣球）。
* [ ] 優化攝影機的遮擋剔除 (Occlusion Culling)，當牆壁擋住主角時自動透明化。
