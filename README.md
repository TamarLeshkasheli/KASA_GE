# კასა.გე
ფისკალური პრინტერის პროტოკოლის (დრაივერის) იმპლემენტაცია.

ბრძანებების დეტალური დოკუმენტაცია!
[GitHub](https://github.com/Antares007/KASA_GE/blob/master/docs/FPCommands.html)


## Initialization:
```
var datec = new Datecs();
var r = datec.InitDp25("com10");
r
// r - არის  სტრინგ არრაის: string[]
// r[0] უდრის "0" - ს თუ ყველაფერმა კარგად ჩაიარა ან უდრის "შეცდომა/განმარტება" - ს.

```
## Z რეპორტის ბეჭდვა:
```
r = datec.Exec(69, "Z");
```
## ფისკალური ჩეკის ბეჭდვა:
```
r = datec.Exec(48, "1", "1", "1", "0");
r = datec.Exec(49, "ყველი", "1", "25.00", "1", "", "", "1");
r = datec.Exec(49, "პური", "1", "25.00", "1", "", "", "1");
r = datec.Exec(49, "ხაჭაპური", "1", "25.00", "1", "", "", "1");
r = datec.Exec(49, "წიწილა", "1", "25.00", "1", "", "", "1");
r = datec.Exec(51, "1", "", "", "");
r = datec.Exec(53, "", "");
r = datec.Exec(56);
```