# Unity Shop Scene — Test Task
A simple Unity shop scene created for the PsyCurio technical task.

## Tech Stack
- Unity 6.4 (6000.4.6f1)
- Universal Render Pipeline (URP)
- C#
- Android build support (.apk)

## Features
### Core Gameplay
- Click a shelf item to place a copy on the counter (maximum 5 items)
- Click a counter item to remove it from the counter
- Click the cash register to see selected items and total price in a speech balloon
- Speech balloon updates live as items are added or removed
- Pay & Leave button to confirm purchase and trigger animated checkout sequence
- Persistent dialogue: once activated, speech balloon stays visible until all the items are removed or purchase is confirmed
- Click the seller character to trigger waving animation

### Visuals
- 3D models for all shelf items:
  - Crown
  - Laughing Mask
  - Magic Wand
  - Sword
  - Mirror
- 3D cash register model
- PBR textures for:
  - Brick walls
  - Gravel floor
  - Wooden counter
- Mixamo character with idle and waving animations
- Breathing green light on cash register when items are on the counter

## Controls
All interactions use mouse clicks:
- Click shelf items to add them to the counter
- Click counter items to remove them
- Click the cash register to open the checkout balloon
- Click Pay & Leave to complete the purchase
- Click the seller character to wave

## Unit Tests
EditMode tests are located in:
```text
Assets/_PsyCurioTask/Tests/EditMode/
```
Run tests via:
```text
Window → General → Test Runner
```
Implemented tests:

| Test | Description |
| ----- | ------------- |
| CounterIncrementsOnAdd | Counter increases by 1 when an item is added |
| CounterDoesNotExceedMax | Counter never exceeds the maximum of 5 items |
| TotalPriceForAllItems | Total price is calculated correctly for all 5 items |
| CheckoutClearsCounter | Checkout resets both item count and total price |
| CanAddItemsAfterCheckout | Items can be added again after checkout |

## How to Run
1. Open the project in Unity 6.4
2. Open:
```text
   Assets/_PsyCurioTask/Scenes/ShopScene
```
3. Press Play

## How to Build
1. Open:
```text
   File → Build Profiles
```
2. Select Android
3. Click Build
