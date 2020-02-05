using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KModkit;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Krit4CardMonte : MonoBehaviour
{
    public KMSelectable Card1Sel, Card2Sel, Card3Sel, Card4Sel, Coin1Sel, Coin10Sel, Coin100Sel, Coin250Sel;
    public KMSelectable DealBtn;
    public KMSelectable PayDevKey1, PayDevKey2, PayDevKey3, PayDevKey4, PayDevKey5, PayDevKey6, PayDevKey7, PayDevKey8, PayDevKey9, PayDevKey0;
    public KMSelectable PayDevKeyReset, PayDevKeySubmit, PayDevKeyCents, PayDevKeyShutdown;
    public KMSelectable DisabledKey;

    public Renderer[] AllCards;
    public Renderer[] AllCoins;

    public Texture Coin1Texture, Coin10Texture, Coin100Texture, Coin250Texture;
    public List<Texture> AllPossibleCards;

    List<string> AllPossibleCardValues = new List<string>
    {
        "Ace Of Spades", "King Of Spades", "Queen Of Spades", "Jack Of Spades", "Ace Of Clubs", "King Of Clubs", "Queen Of Clubs", "Jack Of Clubs", "Ace Of Hearts", "King Of Hearts", "Queen Of Hearts", "Jack Of Hearts", "Ace Of Diamonds", "King Of Diamonds", "Queen Of Diamonds", "Jack Of Diamonds"
    };
    List<string> ShutdownMessages = new List<string>
    {
        "Initiating force shutdown sequence...", "Turning off the lights...", "Ending current session...", "Terminating the program...", "Resetting the neo world...", "Shutting down..."
    };
    List<string> VictoryMessages = new List<string>
    {
        "Well done...",
        "You win!",
        "A winner" + Environment.NewLine + "is you!",
        "We did it" + Environment.NewLine + "Reddit!",
        "You have" + Environment.NewLine + "bested me."
    };
    List<string> Royal_FlushModules = new List<string>
    {
        "Accumulation", "Algebra", "Alphabet Numbers", "Benedict Cumberbatch", "Blockbusters", "British Slang", "Broken Guitar Chords", "Catchphrase", "Christmas Presents" , "Coffeebucks", "Countdown", "Cruel Countdown", "The Crystal Maze", "The Cube", "European Travel", "The Festive Jukebox", "Flashing Lights", "Free Parking", "Graffiti Numbers", "Guitar Chords", "The Hangover", "Hieroglyphics", "Homophones", "Horrible Memory", "Identity Parade", "The iPhone", "The Jack-O'-Lantern", "The Jewel Vault","The Jukebox", "The Labyrinth", "LED Grid", "Lightspeed", "The London Underground", "Maintenance", "Modulo", "The Moon", "Mortal Kombat", "The Number Cipher", "The Plunger Button", "Poker", "Quintuples", "Retirement", "Reverse Morse", "Simon's Stages", "Simon's Star", "Skinny Wires", "Skyrim", "Snooker", "Sonic & Knuckles", "Sonic The Hedgehog", "The Sphere", "Spinning Buttons", "The Stock Market", "The Stopwatch", "Street Fighter", "The Sun", "The Swan", "Symbolic Coordinates", "Tax Returns", "The Triangle", "The Troll", "T-Words", "Westeros", "The Wire", "Wire Spaghetti", "The Matrix", "Stained Glass", "Simon's on First", "Weird Al Yankovic"
    };

    public GameObject Card1Obj, Card2Obj, Card3Obj, Card4Obj;
    public GameObject CardHighlight1, CardHighlight2, CardHighlight3, CardHighlight4;
    public GameObject Coins;
    public GameObject PaymentDevice;
    public GameObject Ticket;
    public GameObject PaymentText;

    public TextMesh StatusText;
    public TextMesh CardNumberText;
    public TextMesh DollarText, CentsText;
    public TextMesh TotalEarningsTicket, CardNumberTicket, VictoryMessage;
    public TextMesh VictoryText;

    public KMBombInfo BombInfo;

    public KMAudio TicketPrinting;

    public KMBombModule ThisModule;

    public KMBossModule bossModule;

    public List<string> CardValues, CardSuits;
    public string[] AllCoinColors;
    public List<string> AllModules;
    public string CardCombo;
    string AllIndicators;
    private string[] ignoredModules;

    public int[] AllCoinValues;
    public int CorrectCard = 0;
    int CardGenerator;
    public int CorrectCoin;
    int FrameCard1 = 0, FrameCard2 = 0, FrameCard3 = 0, FrameCard4 = 0, TextFrame = 0;
    public int TextFrame1 = 0;
    int FrameShuffle1 = 0, FrameShuffle2 = 0;
    int DealTextFrame = 0, DealFrame1 = 0, DealFrame2 = 0, DealFrame3 = 0, DealFrame4 = 0;
    int CoinFrame = 0;
    int CurrentChar = 1;
    public int DesiredDollars;
    public int DesiredCent1, DesiredCent2;
    public int[] AllCardNumbers;
    int InitialTimer;
    int Hour = DateTime.Now.Hour;
    int Day = DateTime.Now.Day;
    //Module ID
    static int moduleIdCounter = 1;
    int ModuleID;

    string CardNumber;
    string Dollars = "000";
    string Cents = "00";

    float CurrentPos1, CurrentPos2, CurrentPos3, CurrentPos4;

    bool InputtingCents = false;
    bool DealAgain = false;

    public readonly string TwitchHelpMessage = "deal [press deal] | coin <#> [select a coin from left to right] | card <#> [select a card from left to right] | send <###.##> [send an amount of money]";
    IEnumerable<KMSelectable> ProcessTwitchCommand(string command)
    {
        string[] split = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        if (split.Length == 1 && split[0] == "deal" && DealBtn.gameObject.activeInHierarchy)
        {
            yield return DealBtn;
        }
        else if (split.Length == 2)
        {
            int position;
            decimal amount;
            if ((split[0] == "coin" || split[0] == "card") && int.TryParse(split[1], out position) && position >= 1 && position <= 4)
            {
                if (split[0] == "coin" && Coins.activeInHierarchy)
                {
                    yield return new[] { Coin1Sel, Coin10Sel, Coin100Sel, Coin250Sel }[position - 1];
                }
                else if (split[0] == "card" && CardHighlight4.activeInHierarchy)
                {
                    yield return new[] { Card1Sel, Card2Sel, Card3Sel, Card4Sel }.OrderBy(selectable => selectable.transform.localPosition.x).ToArray()[position - 1];
                }
            }
            else if (split[0] == "send" && decimal.TryParse(split[1], out amount) && amount <= 999.99m && amount >= 0 && PaymentDevice.activeInHierarchy)
            {
                yield return PayDevKeyReset;

                foreach (char character in amount.ToString("0.00"))
                {
                    if (character == '.')
                    {
                        if (!InputtingCents)
                            yield return PayDevKeyCents;
                    }
                    else
                    {
                        yield return new[] { PayDevKey0, PayDevKey1, PayDevKey2, PayDevKey3, PayDevKey4, PayDevKey5, PayDevKey6, PayDevKey7, PayDevKey8, PayDevKey9 }[character - '0'];
                    }
                }

                yield return PayDevKeySubmit;
            }
        }

        yield break;
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (((int)(BombInfo.GetTime() / 60) % 2 != 0) || ((AllModules.Count() >= 5) && !(BombInfo.GetSolvedModuleNames().Count() >= 5))) yield return true;
        yield return InteractSelectables(ProcessTwitchCommand("deal"));

        while (!Coins.activeInHierarchy) yield return true;
        yield return InteractSelectables(ProcessTwitchCommand("coin " + CorrectCoin));

        while (!CardHighlight4.activeInHierarchy) yield return true;
        // This could just call KMSelectable.OnInteract(), but it helps test the command processor.
        int cardIndex = new[] { Card1Sel, Card2Sel, Card3Sel, Card4Sel }.OrderBy(selectable => selectable.transform.localPosition.x).ToList().IndexOf(new[] { Card1Sel, Card2Sel, Card3Sel, Card4Sel }[CorrectCard - 1]) + 1;

        yield return InteractSelectables(ProcessTwitchCommand("card " + cardIndex));

        while (CardNumberText.text != CardNumber) yield return true;
        while (new[] { "Silly Slots", "Poker", "Point Of Order", "Blackjack" }.Any(module => BombInfo.GetSolvableModuleNames().Count(name => name.Contains(module)) != BombInfo.GetSolvedModuleNames().Count(name => name.Contains(module)))) yield return true;

        yield return InteractSelectables(ProcessTwitchCommand("send " + DesiredDollars + "." + DesiredCent1 + DesiredCent2));
    }

    IEnumerator InteractSelectables(IEnumerable<KMSelectable> selectables)
    {
        foreach (KMSelectable selectable in selectables)
        {
            selectable.OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Start()
    {
        ignoredModules = bossModule.GetIgnoredModules("Four-Card Monte", new string[]
        {
            "Cookie Jars",
            "Cruel Purgatory",
            "Divided Squares",
            "Forget Enigma",
            "Forget Everything",
            "Forget Me Not",
            "Forget Perspective",
            "Forget Them All",
            "Forget This",
            "Forget Us Not",
            "Four-Card Monte",
            "Hogwarts",
            "Purgatory",
            "Simon's Stages",
            "Souvenir",
            "Tallordered Keys",
            "The Swan",
            "The Time Keeper",
            "Timing is Everything",
            "The Troll",
            "Turn The Key"
        });
        ModuleID = moduleIdCounter++;
        AllModules = BombInfo.GetSolvableModuleNames();
        ThisModule.OnActivate = delegate
        {
            AllModules.RemoveAll(u => ignoredModules.Contains(u));
            InitialTimer = (int)BombInfo.GetTime();
        };

        PaymentDevice.SetActive(false);

        CardHighlight1.SetActive(false);
        CardHighlight2.SetActive(false);
        CardHighlight3.SetActive(false);
        CardHighlight4.SetActive(false);

        Card1Sel.OnInteract = InactiveCard;
        Card2Sel.OnInteract = InactiveCard;
        Card3Sel.OnInteract = InactiveCard;
        Card4Sel.OnInteract = InactiveCard;

        Coins.SetActive(false);

        Coin1Sel.OnInteract = Coin1;
        Coin10Sel.OnInteract = Coin2;
        Coin100Sel.OnInteract = Coin3;
        Coin250Sel.OnInteract = Coin4;

        PayDevKey1.OnInteract = PayDevKey1Press;
        PayDevKey2.OnInteract = PayDevKey2Press;
        PayDevKey3.OnInteract = PayDevKey3Press;
        PayDevKey4.OnInteract = PayDevKey4Press;
        PayDevKey5.OnInteract = PayDevKey5Press;
        PayDevKey6.OnInteract = PayDevKey6Press;
        PayDevKey7.OnInteract = PayDevKey7Press;
        PayDevKey8.OnInteract = PayDevKey8Press;
        PayDevKey9.OnInteract = PayDevKey9Press;
        PayDevKey0.OnInteract = PayDevKey0Press;

        PayDevKeyReset.OnInteract = PayDevKeyResetPress;
        PayDevKeySubmit.OnInteract = PayDevKeySubmitPress;
        PayDevKeyCents.OnInteract = PayDevKeyCentPress;

        PayDevKeyShutdown.OnInteract = ShutdownSequence;

        DealBtn.OnInteract = Deal;

        int CurrentCard = 0;
        int CardsLeft = 16;
        foreach (Renderer Card in AllCards)
        {
            CardGenerator = Random.Range(0, CardsLeft);
            Card.material.mainTexture = AllPossibleCards[CardGenerator];
            CardValues[CurrentCard] = AllPossibleCardValues[CardGenerator];

            AllPossibleCards.Remove(Card.material.mainTexture);
            AllPossibleCardValues.Remove(CardValues[CurrentCard]);

            if (CardGenerator < 4)
            {
                //Suit is Spades
                CardSuits[CurrentCard] = "Spades";
            }
            else if (CardGenerator >= 4 && CardGenerator < 8)
            {
                //Suit is Clubs
                CardSuits[CurrentCard] = "Clubs";
            }
            else if (CardGenerator >= 8 && CardGenerator < 12)
            {
                //Suit is Hearts
                CardSuits[CurrentCard] = "Hearts";
            }
            else if (CardGenerator >= 12)
            {
                //Suit is Diamonds
                CardSuits[CurrentCard] = "Diamond";
            }

            CurrentCard++;
            CardsLeft--;
        }
        Debug.LogFormat("[Four-Card Monte #{0}] Your cards are: {1}, {2}, {3} and {4}", ModuleID, CardValues[0], CardValues[1], CardValues[2], CardValues[3]);
        StartCoroutine("DealText");

        if (DateTime.Now.ToString("MM-dd") == "07-07")
        {
            Debug.LogFormat("<Four-Card Monte #{0}> It is now the Lucky Date 07-07. The button will now be activated.", ModuleID);
            DisabledKey.OnInteract = EnabledKey;
        }
    }

    protected bool EnabledKey()
    {
        StartCoroutine(Surprise());
        DisabledKey.OnInteract = null;
        return false;
    }

    IEnumerator Surprise()
    {
        for (int T = 0; T < 120; T++)
        {
            CardNumber = "";
            for (int i = 0; i < 10; i++)
            {
                int CurrentNumber = 0;
                int Digit = 0;
                Digit = Random.Range(0, 10);
                CardNumber += Digit.ToString();
                AllCardNumbers[CurrentNumber] = Digit;
                CurrentNumber++;

                if (T == 0 || T == 7 || T == 14 || T == 21 || T == 28 || T == 35 || T == 42 || T == 49 || T == 56 || T == 63 || T == 70 || T == 77 || T == 84 || T == 91 || T == 98 || T == 105 || T == 112)
                    CardNumberText.color = Color.Lerp(new Color32(255, 0, 0, 255), new Color32(255, 165, 0, 255), 0.01f);
                else if (T == 1 || T == 8 || T == 15 || T == 22 || T == 29 || T == 36 || T == 43 || T == 50 || T == 57 || T == 64 || T == 71 || T == 78 || T == 85 || T == 92 || T == 99 || T == 106 || T == 113)
                    CardNumberText.color = Color.Lerp(new Color32(255, 165, 0, 255), new Color32(255, 255, 0, 255), 0.01f);
                else if (T == 2 || T == 9 || T == 16 || T == 23 || T == 30 || T == 37 || T == 44 || T == 51 || T == 58 || T == 65 || T == 72 || T == 79 || T == 86 || T == 93 || T == 100 || T == 107 || T == 114)
                    CardNumberText.color = Color.Lerp(new Color32(255, 255, 0, 255), new Color32(0, 255, 0, 255), 0.01f);
                else if (T == 3 || T == 10 || T == 17 || T == 24 || T == 31 || T == 38 || T == 45 || T == 52 || T == 59 || T == 66 || T == 73 || T == 80 || T == 87 || T == 94 || T == 101 || T == 108 || T == 115)
                    CardNumberText.color = Color.Lerp(new Color32(0, 255, 0, 255), new Color32(0, 0, 255, 255), 0.01f);
                else if (T == 4 || T == 11 || T == 18 || T == 25 || T == 32 || T == 39 || T == 46 || T == 53 || T == 60 || T == 67 || T == 74 || T == 81 || T == 88 || T == 95 || T == 102 || T == 109 || T == 116)
                    CardNumberText.color = Color.Lerp(new Color32(0, 0, 255, 255), new Color32(255, 0, 255, 255), 0.01f);
                else if (T == 5 || T == 12 || T == 19 || T == 26 || T == 33 || T == 40 || T == 47 || T == 54 || T == 61 || T == 68 || T == 75 || T == 82 || T == 89 || T == 96 || T == 103 || T == 110 || T == 117)
                    CardNumberText.color = Color.Lerp(new Color32(255, 0, 255, 255), new Color32(255, 20, 147, 255), 0.01f);
                else if (T == 6 || T == 13 || T == 20 || T == 27 || T == 34 || T == 41 || T == 48 || T == 55 || T == 62 || T == 69 || T == 76 || T == 83 || T == 90 || T == 97 || T == 104 || T == 111 || T == 118)
                    CardNumberText.color = Color.Lerp(new Color32(255, 20, 147, 255), new Color32(255, 0, 0, 255), 0.01f);

            }
            CardNumberText.text = CardNumber;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        CardNumber = "7777777777";
        CardNumberText.color = Color.white;
        CardNumberText.text = CardNumber;
        Debug.LogFormat("[Four-Card Monte #{0}] It's your lucky day!", ModuleID);
        Debug.LogFormat("[Four-Card Monte #{0}] Card Number is now {1}", ModuleID, CardNumber);
    }

    IEnumerator DealText()
    {
        while (true)
        {
            StatusText.color = Color.white;
            if (DealTextFrame == 1)
            {
                StatusText.text = "Press \"Deal\"";
            }
            if (DealTextFrame == 2)
            {
                StatusText.text = "When ready.";
                DealTextFrame = 0;
            }
            DealTextFrame++;
            yield return new WaitForSecondsRealtime(1);
        }
    }

    protected bool Deal()
    {
        Debug.LogFormat("[Four-Card Monte #{0}] Pressed \"Deal\"", ModuleID);
        DealBtn.AddInteractionPunch();
        if ((int)(BombInfo.GetTime() / 60) % 2 == 0)
        {
            if (AllModules.Count() >= 5)
            {
                if (BombInfo.GetSolvedModuleNames().Count() >= 5)
                {
                    Debug.Log("Current bomb time: " + (int)BombInfo.GetTime() + ". Below half? " + ((int)BombInfo.GetTime() == InitialTimer));
                    if (!DealAgain)
                    {
                        DealBtn.gameObject.SetActive(false);
                        Coins.SetActive(true);
                        StopCoroutine("DealText");
                        StartCoroutine("DealCoins");
                        CoinGenerator();
                        CorrectCardCalculation();
                        StartCoroutine("DealCard1");
                    }
                    else
                    {
                        DealBtn.gameObject.SetActive(false);
                        Coins.SetActive(true);
                        StopCoroutine("DealText");
                        StartCoroutine("DealCoins");
                        CoinGenerator();
                        CorrectCardCalculation();
                        StartCoroutine("DealCard1Again");
                    }
                }
                else
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Couldn't deal now, because the solve count ({1}) is below 5", ModuleID, BombInfo.GetSolvedModuleNames().Count());
                    StatusText.color = Color.red;
                    StatusText.text = "CAN'T DEAL NOW";
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                if (!DealAgain)
                {
                    DealBtn.gameObject.SetActive(false);
                    Coins.SetActive(true);
                    StopCoroutine("DealText");
                    StartCoroutine("DealCoins");
                    CoinGenerator();
                    CorrectCardCalculation();
                    StartCoroutine("DealCard1");
                }
                else
                {
                    DealBtn.gameObject.SetActive(false);
                    Coins.SetActive(true);
                    StopCoroutine("DealText");
                    StartCoroutine("DealCoins");
                    CoinGenerator();
                    CorrectCardCalculation();
                    StartCoroutine("DealCard1Again");
                }
            }
        }
        else
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Couldn't deal now, because the time was incorrect", ModuleID);
            StatusText.color = Color.red;
            StatusText.text = "Can't deal now";
            GetComponent<KMBombModule>().HandleStrike();
        }
        return false;
    }

    IEnumerator DealCoins()
    {
        float UpwardsMovement = 0.102f;
        int Rotation = 0;
        while (true)
        {
            if (CoinFrame < 11)
            {
                Coin1Sel.gameObject.transform.localPosition = new Vector3(-0.0339f, UpwardsMovement, -0.0009f);
                Coin10Sel.gameObject.transform.localPosition = new Vector3(0.001800001f, UpwardsMovement, -0.0137f);
                Coin100Sel.gameObject.transform.localPosition = new Vector3(0.0344f, UpwardsMovement, 0.0083f);
                Coin250Sel.gameObject.transform.localPosition = new Vector3(0.058f, UpwardsMovement, -0.0159f);
                Coin1Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin10Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin100Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin250Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                UpwardsMovement = UpwardsMovement - 0.004f;
                Rotation = Rotation + 36;
            }
            if (CoinFrame == 11)
            {
                UpwardsMovement = 0.07000002f;
            }
            if (CoinFrame >= 11 && CoinFrame < 16)
            {
                Coin1Sel.gameObject.transform.localPosition = new Vector3(-0.0339f, UpwardsMovement, -0.0009f);
                Coin10Sel.gameObject.transform.localPosition = new Vector3(0.001800001f, UpwardsMovement, -0.0137f);
                Coin100Sel.gameObject.transform.localPosition = new Vector3(0.0344f, UpwardsMovement, 0.0083f);
                Coin250Sel.gameObject.transform.localPosition = new Vector3(0.058f, UpwardsMovement, -0.0159f);
                Coin1Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin10Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin100Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin250Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                UpwardsMovement = UpwardsMovement + 0.002f;
                Rotation = Rotation + 36;
            }
            if (CoinFrame >= 16 && CoinFrame < 21)
            {
                Coin1Sel.gameObject.transform.localPosition = new Vector3(-0.0339f, UpwardsMovement, -0.0009f);
                Coin10Sel.gameObject.transform.localPosition = new Vector3(0.001800001f, UpwardsMovement, -0.0137f);
                Coin100Sel.gameObject.transform.localPosition = new Vector3(0.0344f, UpwardsMovement, 0.0083f);
                Coin250Sel.gameObject.transform.localPosition = new Vector3(0.058f, UpwardsMovement, -0.0159f);
                Coin1Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin10Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin100Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                Coin250Sel.gameObject.transform.localEulerAngles = new Vector3(0, 0, Rotation);
                UpwardsMovement = UpwardsMovement - 0.002f;
                Rotation = Rotation + 36;
            }
            CoinFrame++;
            yield return new WaitForSecondsRealtime(0.025f);
        }
    }

    void CoinGenerator()
    {
        int CurrentCoin = 0;
        foreach (Renderer Coin in AllCoins)
        {
            int CoinGen = Random.Range(0, 4);
            switch (CoinGen)
            {
                case 0:
                    {
                        Coin.material.mainTexture = Coin1Texture;
                        AllCoinColors[CurrentCoin] = "Red Coin";
                        AllCoinValues[CurrentCoin] = 1;
                        break;
                    }
                case 1:
                    {
                        Coin.material.mainTexture = Coin10Texture;
                        AllCoinColors[CurrentCoin] = "Blue Coin";
                        AllCoinValues[CurrentCoin] = 10;
                        break;
                    }
                case 2:
                    {
                        Coin.material.mainTexture = Coin100Texture;
                        AllCoinColors[CurrentCoin] = "Green Coin";
                        AllCoinValues[CurrentCoin] = 100;
                        break;
                    }
                case 3:
                    {
                        Coin.material.mainTexture = Coin250Texture;
                        AllCoinColors[CurrentCoin] = "Black Coin";
                        AllCoinValues[CurrentCoin] = 250;
                        break;
                    }
            }
            CurrentCoin++;
        }
        CorrectCoinCalculation();
    }

    void CorrectCardCalculation()
    {
        if (CardValues.All(x => x.Contains("Spades")) || CardValues.All(x => x.Contains("Hearts")) || CardValues.All(x => x.Contains("Clubs")) || CardValues.All(x => x.Contains("Diamonds")))
        {
            if (CardValues.Count(x => x.Contains("Ace")) == 1 && CardValues.Count(x => x.Contains("King")) == 1 && CardValues.Count(x => x.Contains("Queen")) == 1 && CardValues.Count(x => x.Contains("Jack")) == 1)
            {
                CardCombo = "Four-Card Deluxe";
            }
            else
            {
                CardCombo = "Four Flush";
            }
        }
        else if (CardValues.Count(x => x.Contains("Spades")) == 3 || CardValues.Count(x => x.Contains("Hearts")) == 3 || CardValues.Count(x => x.Contains("Diamonds")) == 3 || CardValues.Count(x => x.Contains("Clubs")) == 3)
        {
            CardCombo = "Three of a suit";
        }
        else if (CardValues.Count(x => x.Contains("King")) == 1 && CardValues.Count(x => x.Contains("Jack")) == 2)
        {
            if (CardValues.Count(x => x.Contains("Queen")) == 1)
            {
                CardCombo = "Kingdom Combo";
            }
            else
            {
                CardCombo = "Royalty Rush";
            }
        }
        else if (CardValues.Count(x => x.Contains("Ace")) > 1)
        {
            CardCombo = "Aces High";
        }
        else if (CardValues.Count(x => x.Contains("Queen")) == 1 && CardValues.Count(x => x.Contains("Ace")) == 1)
        {
            CardCombo = "Queen's Rule";
        }
        else if (CardSuits.Distinct().Count() == 2)
        {
            CardCombo = "Dual Pairs";
        }
        else if (CardValues.Count(x => x.Contains("Hearts")) == 2)
        {
            CardCombo = "Lucky Love";
        }
        else
        {
            CardCombo = "Total Trash";
        }

        Debug.LogFormat("[Four-Card Monte #{0}] Your hand: {1}", ModuleID, CardCombo);
        FlowchartCalculation();
    }

    void FlowchartCalculation()
    {
        AllIndicators = string.Join("", BombInfo.GetIndicators().ToArray());
        //Flowchart: Four-Card Deluxe and Three of a suit
        if (CardCombo == "Four-Card Deluxe" || CardCombo == "Three of a suit")
        {
            if (CardValues[0].Contains("Spades"))
            {
                if (BombInfo.GetSerialNumberLetters().Any("AEIOU".Contains))
                {
                    if (CardValues.Count(x => x.Contains("Spades")) >= 2)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (CardValues.Count(x => x.Contains("Hearts")) >= 2)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
            else
            {
                if (AllIndicators.Any("AEIOU".Contains))
                {
                    if (CardValues.Count(x => x.Contains("Diamonds")) >= 2)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (CardValues.Count(x => x.Contains("Clubs")) >= 2)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
        }

        //Flowchart: Four Flush
        else if (CardCombo == "Four Flush")
        {
            if (BombInfo.GetSerialNumberNumbers().First() > 5)
            {
                if (BombInfo.IsIndicatorPresent(Indicator.BOB))
                {
                    if (BombInfo.GetBatteryCount() > 2)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (BombInfo.GetPortCount() > 2)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
            else
            {
                if (BombInfo.IsIndicatorPresent(Indicator.TRN))
                {
                    if (BombInfo.GetIndicators().Count() > 2)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (BombInfo.GetSerialNumberNumbers().Last() < 5)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
        }

        //Flowchart: Kingdom Combo and Royalty Rush
        else if (CardCombo == "Kingdom Combo" || CardCombo == "Royalty Rush")
        {
            if (AllModules.Any(x => Royal_FlushModules.Any(y => y == x)))
            {
                if (AllModules.Any(x => x.Contains("Poker")))
                {
                    if (AllModules.Any(x => x.Contains("Modulo")))
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (AllModules.Any(x => x.Contains("British Slang")))
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
            else
            {
                if (BombInfo.GetSolvedModuleNames().Count() > 7)
                {
                    if (AllModules.Any(x => x.Contains("Flip The Coin")))
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (AllModules.Any(x => x.Contains("Blackjack")))
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
        }

        //Flowchart: Aces High and Queen's Rule
        else if (CardCombo == "Aces High" || CardCombo == "Queen's Rule")
        {
            if (CardValues.Count(x => x.Contains("Ace")) == 1 && CardValues.Count(x => x.Contains("King")) == 1 && CardValues.Count(x => x.Contains("Queen")) == 1 && CardValues.Count(x => x.Contains("Jack")) == 1)
            {
                if (CardValues.Count(x => x.Contains("Hearts")) == 2)
                {
                    if (CardValues.Count(x => x.Contains("Queen")) > 0)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (CardValues.Count(x => x.Contains("Jack")) > 0)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
            else
            {
                if (CardValues.Count(x => x.Contains("Spades")) == 2)
                {
                    if (CardValues.Count(x => x.Contains("Ace")) > 0)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (CardValues.Count(x => x.Contains("King")) > 0)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
        }

        //Flowchart: Lucky Love and Daul Pairs
        else if (CardCombo == "Lucky Love" || CardCombo == "Dual Pairs")
        {
            if (BombInfo.GetSolvedModuleNames().Count() > BombInfo.GetSolvableModuleNames().Count() / 2)
            {
                if (BombInfo.GetTime() < InitialTimer / 2)
                {
                    if (AllCoinValues[CorrectCoin - 1] == 1)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (AllCoinValues[CorrectCoin - 1] == 10)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
            else
            {
                if (BombInfo.GetPortCount() > BombInfo.GetSolvableModuleNames().Count())
                {
                    if (AllCoinValues[CorrectCoin - 1] == 100)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (AllCoinValues[CorrectCoin - 1] == 250)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
        }

        //Flowchart: Total Trash
        else
        {
            if (Hour >= 0 && Hour < 12)
            {
                if (Day % 2 != 0)
                {
                    if (BombInfo.GetTime() >= InitialTimer / 2)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (BombInfo.GetTime() < InitialTimer / 2)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
            else
            {
                if (Day % 2 == 0)
                {
                    if (BombInfo.GetTime() >= 300)
                    {
                        CorrectCard = 1;
                    }
                    else
                    {
                        CorrectCard = 2;
                    }
                }
                else
                {
                    if (BombInfo.GetTime() < 300)
                    {
                        CorrectCard = 3;
                    }
                    else
                    {
                        CorrectCard = 4;
                    }
                }
            }
        }
        Debug.LogFormat("[Four-Card Monte #{0}] The correct card is {1}", ModuleID, CorrectCard);
    }

    void CorrectCoinCalculation()
    {
        string Rule;
        if (CardValues[0] == "Ace Of Spades" && BombInfo.IsIndicatorOn(Indicator.BOB))
        {
            Rule = "First card is Ace of Spades and BOB is lit";
            CorrectCoin = 1;
        }
        if (CardValues[3] == "Jack Of Clubs" && AllCoinColors.Where(x => x.Contains("Red")).Count() > 1)
        {
            Rule = "Last card is Jack Of Clubs and 2-4 red coins";
            CorrectCoin = 4;
        }
        else if (CardValues.Any(x => x.Contains("Queen Of Hearts")) && CardValues.Any(x => x.Contains("King")))
        {
            Rule = "Both a Queen of Hearts and a King are present";
            CorrectCoin = 2;
        }
        else if (CardValues.Any(x => x.Contains("Ace Of Diamonds")) && AllCoinColors.Count() == AllCoinColors.Distinct().Count())
        {
            Rule = "Ace Of Diamonds is present and there are no duplicate coins";
            CorrectCoin = 3;
        }
        else if (CardValues.Any(x => x.Contains("Spades")) && CardValues.Any(x => x.Contains("Clubs")) && CardValues.Any(x => x.Contains("Hearts")) && CardValues.Any(x => x.Contains("Diamonds")))
        {
            Rule = "All cards are from different suits";
            CorrectCoin = 1;
        }
        else if (CardValues.Count(x => x.Contains("Spades")) == 2 && CardValues.Count(x => x.Contains("Clubs")) == 2)
        {
            Rule = "2 Spades and 2 Clubs present";
            CorrectCoin = 2;
        }
        else if (CardValues.Count(x => x.Contains("Hearts")) == 2 && CardValues.Count(x => x.Contains("Diamonds")) == 2)
        {
            Rule = "2 Hearts and 2 Diamonds present";
            CorrectCoin = 4;
        }
        else if (CardValues.Count(x => x.Contains("Spades")) == 2 && CardValues.Count(x => x.Contains("Hearts")) == 2)
        {
            Rule = "You have 2 pairs";
            CorrectCoin = 3;
        }
        else if (CardValues.Count(x => x.Contains("Clubs")) == 2 && CardValues.Count(x => x.Contains("Diamonds")) == 2)
        {
            Rule = "You have 2 pairs";
            CorrectCoin = 3;
        }
        else if (CardValues.Count(x => x.Contains("Hearts")) == 2 && CardValues.Count(x => x.Contains("Clubs")) == 2)
        {
            Rule = "You have 2 pairs";
            CorrectCoin = 3;
        }
        else if (CardValues.Count(x => x.Contains("Spades")) == 2 && CardValues.Count(x => x.Contains("Diamonds")) == 2)
        {
            Rule = "You have 2 pairs";
            CorrectCoin = 3;
        }
        else if (BombInfo.GetSerialNumberLetters().Any("AEIOU".Contains))
        {
            int SNOffset = BombInfo.GetSerialNumberNumbers().Last();
            while (SNOffset > 4)
            {
                SNOffset = SNOffset - 4;
            }
            if (SNOffset == 0)
            {
                SNOffset = SNOffset + 4;
            }
            CorrectCoin = SNOffset;
            Rule = "None of the rules apply, but the serial number contains a vowel";
        }
        else
        {
            int SNOffset = BombInfo.GetSerialNumberNumbers().First();
            while (SNOffset > 4)
            {
                SNOffset = SNOffset - 4;
            }
            if (SNOffset == 0)
            {
                SNOffset = SNOffset + 4;
            }
            CorrectCoin = SNOffset;
            Rule = "None of the rules apply and the serial doesn't contain a vowel";
        }
        Debug.LogFormat("[Four-Card Monte #{0}] {1}, so the desired coin is coin {2} (${3})", ModuleID, Rule, CorrectCoin, AllCoinValues[CorrectCoin - 1]);
    }

    IEnumerator DealCard1()
    {
        StatusText.text = "Dealing...";
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.067f;
        while (true)
        {
            if (DealFrame1 < 3)
            {
                Card1Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
            }
            if (DealFrame1 == 5)
            {
                StartCoroutine("DealCard2");
            }
            if (DealFrame1 >= 5 && DealFrame1 < 9)
            {
                Card1Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame1 == 9)
            {
                DealFrame1 = 0;
                StopCoroutine("DealCard1");
            }
            DealFrame1++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator DealCard2()
    {
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.0534f;
        while (true)
        {
            if (DealFrame2 < 3)
            {
                Card2Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.01f;
            }
            if (DealFrame2 == 5)
            {
                StartCoroutine("DealCard3");
            }
            if (DealFrame2 >= 5 && DealFrame2 < 9)
            {
                Card2Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame2 == 9)
            {
                DealFrame2 = 0;
                StopCoroutine("DealCard2");
            }
            DealFrame2++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator DealCard3()
    {
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.0534f;
        while (true)
        {
            if (DealFrame3 < 3)
            {
                Card3Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.025f;
            }
            if (DealFrame3 == 5)
            {
                StartCoroutine("DealCard4");
            }
            if (DealFrame3 >= 5 && DealFrame3 < 9)
            {
                Card3Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame3 == 9)
            {
                DealFrame3 = 0;
                StopCoroutine("DealCard3");
            }
            DealFrame3++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator DealCard4()
    {
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.0534f;
        while (true)
        {
            if (DealFrame4 < 3)
            {
                Card4Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.04f;
            }
            if (DealFrame4 >= 5 && DealFrame4 < 9)
            {
                Card4Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame4 == 9)
            {
                DealAgain = true;
                DealFrame4 = 0;
                StatusText.text = "Place a bet";
                StopCoroutine("DealCard4");
            }
            DealFrame4++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }

    IEnumerator DealCard1Again()
    {
        StatusText.text = "Dealing...";
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.067f;
        while (true)
        {
            if (DealFrame1 < 4)
            {
                Card1Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
            }
            if (DealFrame1 == 5)
            {
                StartCoroutine("DealCard2Again");
            }
            if (DealFrame1 >= 4 && DealFrame1 < 8)
            {
                Card1Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame1 == 8)
            {
                DealFrame1 = 0;
                StopCoroutine("DealCard1Again");
            }
            DealFrame1++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator DealCard2Again()
    {
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.0534f;
        while (true)
        {
            if (DealFrame2 < 4)
            {
                Card2Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.01f;
            }
            if (DealFrame2 == 5)
            {
                StartCoroutine("DealCard3Again");
            }
            if (DealFrame2 >= 4 && DealFrame2 < 8)
            {
                Card2Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame2 == 8)
            {
                DealFrame2 = 0;
                StopCoroutine("DealCard2Again");
            }
            DealFrame2++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator DealCard3Again()
    {
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.0534f;
        while (true)
        {
            if (DealFrame3 < 4)
            {
                Card3Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.025f;
            }
            if (DealFrame3 == 5)
            {
                StartCoroutine("DealCard4Again");
            }
            if (DealFrame3 >= 4 && DealFrame3 < 8)
            {
                Card3Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame3 == 8)
            {
                DealFrame3 = 0;
                StopCoroutine("DealCard3Again");
            }
            DealFrame3++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator DealCard4Again()
    {
        float UpwardsMovementCard1 = -0.061f;
        float SidewaysMovementCard1 = -0.0534f;
        while (true)
        {
            if (DealFrame4 < 4)
            {
                Card4Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.04f;
            }
            if (DealFrame4 >= 4 && DealFrame4 < 8)
            {
                Card4Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.001f;
            }
            if (DealFrame4 == 8)
            {
                DealFrame4 = 0;
                StatusText.text = "Place a bet";
                StopCoroutine("DealCard4Again");
            }
            DealFrame4++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }

    protected bool Coin1()
    {
        Coin1Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Coin pressed: 1 (${1}), Desired: {2} (${3})", ModuleID, AllCoinValues[0], CorrectCoin, AllCoinValues[CorrectCoin - 1]);
        if (CorrectCoin == 1)
        {
            Coins.SetActive(false);
            StartCoroutine("ShuffleText");
            if (!DealAgain)
            {
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
                StartCoroutine("FlipCard1");
            }
            else
            {
                StartCoroutine("FlipCard1Again");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
        }
        else
        {
            StatusText.color = Color.red;
            StatusText.text = "Bet declined!";
            Debug.LogFormat("[Four-Card Monte #{0}] Bet was invalid. Strike handed.", ModuleID);
            GetComponent<KMBombModule>().HandleStrike();
        }
        return false;
    }
    protected bool Coin2()
    {
        Coin10Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Coin pressed: 2 (${1}), Desired: {2} (${3})", ModuleID, AllCoinValues[1], CorrectCoin, AllCoinValues[CorrectCoin - 1]);
        if (CorrectCoin == 2)
        {
            Coins.SetActive(false);
            StartCoroutine("ShuffleText");
            if (!DealAgain)
            {
                StartCoroutine("FlipCard1");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
            else
            {
                StartCoroutine("FlipCard1Again");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
        }
        else
        {
            StatusText.color = Color.red;
            StatusText.text = "Bet declined!";
            Debug.LogFormat("[Four-Card Monte #{0}] Bet was invalid. Strike handed.", ModuleID);
            GetComponent<KMBombModule>().HandleStrike();
        }
        return false;
    }
    protected bool Coin3()
    {
        Coin100Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Coin pressed: 3 (${1}), Desired: {2} (${3})", ModuleID, AllCoinValues[2], CorrectCoin, AllCoinValues[CorrectCoin - 1]);
        if (CorrectCoin == 3)
        {
            Coins.SetActive(false);
            StartCoroutine("ShuffleText");
            if (!DealAgain)
            {
                StartCoroutine("FlipCard1");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
            else
            {
                StartCoroutine("FlipCard1Again");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
        }
        else
        {
            StatusText.color = Color.red;
            StatusText.text = "Bet declined!";
            Debug.LogFormat("[Four-Card Monte #{0}] Bet was invalid. Strike handed.", ModuleID);
            GetComponent<KMBombModule>().HandleStrike();
        }
        return false;
    }
    protected bool Coin4()
    {
        Coin250Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Coin pressed: 4 (${1}), Desired: {2} (${3})", ModuleID, AllCoinValues[3], CorrectCoin, AllCoinValues[CorrectCoin - 1]);
        if (CorrectCoin == 4)
        {
            Coins.SetActive(false);
            StartCoroutine("ShuffleText");
            if (!DealAgain)
            {
                StartCoroutine("FlipCard1");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
            else
            {
                StartCoroutine("FlipCard1Again");
                Debug.LogFormat("[Four-Card Monte #{0}] Bet accepted. Shuffling...", ModuleID);
            }
        }
        else
        {
            StatusText.color = Color.red;
            StatusText.text = "Bet declined!";
            Debug.LogFormat("[Four-Card Monte #{0}] Bet was invalid. Strike handed.", ModuleID);
            GetComponent<KMBombModule>().HandleStrike();
        }
        return false;
    }

    IEnumerator ShuffleText()
    {
        while (true)
        {
            if (TextFrame == 0)
            {
                StatusText.color = Color.white;
                StatusText.text = "Shuffling";
            }
            if (TextFrame == 1)
            {
                StatusText.text = "Shuffling.";
            }
            if (TextFrame == 2)
            {
                StatusText.text = "Shuffling..";
            }
            if (TextFrame == 3)
            {
                StatusText.text = "Shuffling...";
                TextFrame = 0;
            }
            TextFrame++;
            yield return new WaitForSecondsRealtime(0.55f);
        }
    }

    IEnumerator FlipCard1()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard1 < 10)
            {
                Card1Obj.transform.localPosition = new Vector3(-0.0622f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard1++;
            }
            if (FrameCard1 == 7)
            {
                StartCoroutine("FlipCard2");
            }
            if (FrameCard1 >= 10 && FrameCard1 < 15)
            {
                Card1Obj.transform.localPosition = new Vector3(-0.0622f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card1Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard1++;
            }
            if (FrameCard1 >= 15 && FrameCard1 < 50)
            {
                Card1Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard1++;
            }
            if (FrameCard1 >= 50 && FrameCard1 < 62)
            {
                Card1Obj.transform.localPosition = new Vector3(-0.0622f, UpwardsMovement, 0.011f);
                Card1Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard1++;
            }
            if (FrameCard1 == 62)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard1 = 0;
                StopCoroutine("FlipCard1");
            }
            yield return new WaitForSecondsRealtime(0.01f);

        }
    }

    IEnumerator FlipCard2()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard2 < 10)
            {
                Card2Obj.transform.localPosition = new Vector3(-0.0206f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard2++;
            }
            if (FrameCard2 == 7)
            {
                StartCoroutine("FlipCard3");
            }
            if (FrameCard2 >= 10 && FrameCard2 < 15)
            {
                Card2Obj.transform.localPosition = new Vector3(-0.0206f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card2Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard2++;
            }
            if (FrameCard2 >= 15 && FrameCard2 < 50)
            {
                Card2Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard2++;
            }
            if (FrameCard2 >= 50 && FrameCard2 < 62)
            {
                Card2Obj.transform.localPosition = new Vector3(-0.0206f, UpwardsMovement, 0.011f);
                Card2Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard2++;
            }
            if (FrameCard2 == 62)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard2 = 0;
                StopCoroutine("FlipCard2");
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator FlipCard3()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard3 < 10)
            {
                Card3Obj.transform.localPosition = new Vector3(0.0209f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard3++;
            }
            if (FrameCard3 == 7)
            {
                StartCoroutine("FlipCard4");
            }
            if (FrameCard3 >= 10 && FrameCard3 < 15)
            {
                Card3Obj.transform.localPosition = new Vector3(0.0209f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card3Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard3++;
            }
            if (FrameCard3 >= 15 && FrameCard3 < 50)
            {
                Card3Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard3++;
            }
            if (FrameCard3 >= 50 && FrameCard3 < 62)
            {
                Card3Obj.transform.localPosition = new Vector3(0.0209f, UpwardsMovement, 0.011f);
                Card3Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard3++;
            }
            if (FrameCard3 == 62)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard3 = 0;
                StopCoroutine("FlipCard3");
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator FlipCard4()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard4 < 10)
            {
                Card4Obj.transform.localPosition = new Vector3(0.0627f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard4++;
            }
            if (FrameCard4 >= 10 && FrameCard4 < 15)
            {
                Card4Obj.transform.localPosition = new Vector3(0.0627f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card4Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard4++;
            }
            if (FrameCard4 >= 15 && FrameCard4 < 50)
            {
                Card4Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard4++;
            }
            if (FrameCard4 >= 50 && FrameCard4 < 62)
            {
                Card4Obj.transform.localPosition = new Vector3(0.0627f, UpwardsMovement, 0.011f);
                Card4Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard4++;
            }
            if (FrameCard4 == 62)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard4 = 0;
                Shuffle1();
                StopCoroutine("FlipCard4");
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator FlipCard1Again()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard1 < 10)
            {
                Card1Obj.transform.localPosition = new Vector3(-0.0622f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard1++;
            }
            if (FrameCard1 == 7)
            {
                StartCoroutine("FlipCard2Again");
            }
            if (FrameCard1 >= 10 && FrameCard1 < 15)
            {
                Card1Obj.transform.localPosition = new Vector3(-0.0622f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card1Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard1++;
            }
            if (FrameCard1 >= 15 && FrameCard1 < 50)
            {
                Card1Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard1++;
            }
            if (FrameCard1 >= 50 && FrameCard1 < 60)
            {
                Card1Obj.transform.localPosition = new Vector3(-0.0622f, UpwardsMovement, 0.011f);
                Card1Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard1++;
            }
            if (FrameCard1 == 60)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard1 = 0;
                StopCoroutine("FlipCard1Again");
            }
            yield return new WaitForSecondsRealtime(0.01f);

        }
    }

    IEnumerator FlipCard2Again()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard2 < 10)
            {
                Card2Obj.transform.localPosition = new Vector3(-0.0206f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard2++;
            }
            if (FrameCard2 == 7)
            {
                StartCoroutine("FlipCard3Again");
            }
            if (FrameCard2 >= 10 && FrameCard2 < 15)
            {
                Card2Obj.transform.localPosition = new Vector3(-0.0206f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card2Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard2++;
            }
            if (FrameCard2 >= 15 && FrameCard2 < 50)
            {
                Card2Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard2++;
            }
            if (FrameCard2 >= 50 && FrameCard2 < 60)
            {
                Card2Obj.transform.localPosition = new Vector3(-0.0206f, UpwardsMovement, 0.011f);
                Card2Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard2++;
            }
            if (FrameCard2 == 60)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard2 = 0;
                StopCoroutine("FlipCard2Again");
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator FlipCard3Again()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard3 < 10)
            {
                Card3Obj.transform.localPosition = new Vector3(0.0209f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard3++;
            }
            if (FrameCard3 == 7)
            {
                StartCoroutine("FlipCard4Again");
            }
            if (FrameCard3 >= 10 && FrameCard3 < 15)
            {
                Card3Obj.transform.localPosition = new Vector3(0.0209f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card3Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard3++;
            }
            if (FrameCard3 >= 15 && FrameCard3 < 50)
            {
                Card3Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard3++;
            }
            if (FrameCard3 >= 50 && FrameCard3 < 60)
            {
                Card3Obj.transform.localPosition = new Vector3(0.0209f, UpwardsMovement, 0.011f);
                Card3Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard3++;
            }
            if (FrameCard3 == 60)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard3 = 0;
                StopCoroutine("FlipCard3Again");
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator FlipCard4Again()
    {
        float UpwardsMovement = 0.0154f;
        float SidewaysMovement = 0;
        while (true)
        {
            if (FrameCard4 < 10)
            {
                Card4Obj.transform.localPosition = new Vector3(0.0627f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                FrameCard4++;
            }
            if (FrameCard4 >= 10 && FrameCard4 < 15)
            {
                Card4Obj.transform.localPosition = new Vector3(0.0627f, UpwardsMovement, 0.011f);
                UpwardsMovement = UpwardsMovement + 0.0025f;
                Card4Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 1);
                SidewaysMovement = SidewaysMovement + 50;
                FrameCard4++;
            }
            if (FrameCard4 >= 15 && FrameCard4 < 50)
            {
                Card4Obj.transform.localRotation = new Quaternion(SidewaysMovement, 180, 0, 10);
                SidewaysMovement = SidewaysMovement + 100;
                FrameCard4++;
            }
            if (FrameCard4 >= 50 && FrameCard4 < 60)
            {
                Card4Obj.transform.localPosition = new Vector3(0.0627f, UpwardsMovement, 0.011f);
                Card4Obj.transform.localRotation = new Quaternion(180, 0, 0, 0);
                UpwardsMovement = UpwardsMovement - 0.00345f;
                FrameCard4++;
            }
            if (FrameCard4 == 60)
            {
                UpwardsMovement = 0.0154f;
                SidewaysMovement = 0;
                FrameCard4 = 0;
                Shuffle1();
                StopCoroutine("FlipCard4Again");
            }
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    void Shuffle1()
    {
        int ShuffleGen = Random.Range(0, 3);
        switch (ShuffleGen)
        {
            case 0:
                {
                    StartCoroutine("FirstCard1With2Shuffle");
                    StartCoroutine("FirstCard3With4Shuffle");
                    break;
                }
            case 1:
                {
                    StartCoroutine("FirstCard1With3Shuffle");
                    StartCoroutine("FirstCard2With4Shuffle");
                    break;
                }
            case 2:
                {
                    StartCoroutine("FirstCard1With4Shuffle");
                    StartCoroutine("FirstCard2With3Shuffle");
                    break;
                }
        }
    }

    IEnumerator FirstCard1With2Shuffle()
    {
        float UpwardsMovementCard1 = -0.0622f;
        float SidewaysMovementCard1 = 0.011f;
        float UpwardsMovementCard2 = -0.0206f;
        float SidewaysMovementCard2 = 0.011f;
        while (true)
        {
            if (FrameShuffle1 < 22)
            {
                Card1Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.00099f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.00095f;
                Card2Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 - 0.00099f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.00095f;
            }
            if (FrameShuffle1 >= 22 && FrameShuffle1 < 45)
            {
                Card1Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.00099f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.00095f;
                Card2Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 + 0.00099f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.00095f;
            }
            if (FrameShuffle1 == 45)
            {
                FrameShuffle1 = 0;
                StopCoroutine("FirstCard1With2Shuffle");
            }
            yield return new WaitForSecondsRealtime(0.005f);
            FrameShuffle1++;
        }
    }

    IEnumerator FirstCard1With3Shuffle()
    {
        float UpwardsMovementCard1 = -0.0622f;
        float SidewaysMovementCard1 = 0.011f;
        float UpwardsMovementCard2 = 0.0209f;
        float SidewaysMovementCard2 = 0.011f;
        while (true)
        {
            if (FrameShuffle1 < 22)
            {
                Card1Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.001f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.0019f;
                Card3Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 - 0.001f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.0019f;
            }
            if (FrameShuffle1 >= 22 && FrameShuffle1 < 45)
            {
                Card1Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.001f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.0019f;
                Card3Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 + 0.001f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.0019f;
            }
            if (FrameShuffle1 == 45)
            {
                FrameShuffle1 = 0;

                StopCoroutine("FirstCard1With3Shuffle");
            }
            yield return new WaitForSecondsRealtime(0.005f);
            FrameShuffle1++;
        }
    }

    IEnumerator FirstCard1With4Shuffle()
    {
        float UpwardsMovementCard1 = -0.0622f;
        float SidewaysMovementCard1 = 0.011f;
        float UpwardsMovementCard2 = 0.0627f;
        float SidewaysMovementCard2 = 0.011f;
        while (true)
        {
            if (FrameShuffle1 < 22)
            {
                Card1Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.00175f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.0028725f;
                Card4Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 - 0.00175f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.0028725f;
            }
            if (FrameShuffle1 >= 22 && FrameShuffle1 < 45)
            {
                Card1Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.00175f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.0028725f;
                Card4Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 + 0.00175f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.0028725f;
            }
            if (FrameShuffle1 == 45)
            {
                FrameShuffle1 = 0;

                StopCoroutine("FirstCard1With4Shuffle");
            }
            yield return new WaitForSecondsRealtime(0.005f);
            FrameShuffle1++;
        }
    }

    IEnumerator FirstCard2With3Shuffle()
    {
        float UpwardsMovementCard1 = -0.0206f;
        float SidewaysMovementCard1 = 0.011f;
        float UpwardsMovementCard2 = 0.0209f;
        float SidewaysMovementCard2 = 0.011f;
        while (true)
        {
            if (FrameShuffle2 < 22)
            {
                Card2Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.00099f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.00095f;
                Card3Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 - 0.00099f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.00095f;
            }
            if (FrameShuffle2 >= 22 && FrameShuffle2 < 45)
            {
                Card2Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.00099f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.00095f;
                Card3Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 + 0.00099f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.00095f;
            }
            if (FrameShuffle2 == 45)
            {
                FrameShuffle2 = 0;
                CardHighlight1.SetActive(true);
                CardHighlight2.SetActive(true);
                CardHighlight3.SetActive(true);
                CardHighlight4.SetActive(true);
                CardHighlight1.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight2.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight3.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight4.transform.localEulerAngles = new Vector3(0, 180, 0);

                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;

                StatusText.text = "Choose one.";
                StopCoroutine("ShuffleText");
                StopCoroutine("FirstCard2With3Shuffle");
            }
            yield return new WaitForSecondsRealtime(0.005f);
            FrameShuffle2++;
        }
    }

    IEnumerator FirstCard2With4Shuffle()
    {
        float UpwardsMovementCard1 = -0.0206f;
        float SidewaysMovementCard1 = 0.011f;
        float UpwardsMovementCard2 = 0.0627f;
        float SidewaysMovementCard2 = 0.011f;
        while (true)
        {
            if (FrameShuffle2 < 22)
            {
                Card2Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.001f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.0019f;
                Card4Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 - 0.001f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.0019f;
            }
            if (FrameShuffle2 >= 22 && FrameShuffle2 < 45)
            {
                Card2Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.001f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.0019f;
                Card4Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 + 0.001f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.0019f;
            }
            if (FrameShuffle2 == 45)
            {
                FrameShuffle2 = 0;
                FrameShuffle2 = 0;
                CardHighlight1.SetActive(true);
                CardHighlight2.SetActive(true);
                CardHighlight3.SetActive(true);
                CardHighlight4.SetActive(true);
                CardHighlight1.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight2.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight3.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight4.transform.localEulerAngles = new Vector3(0, 180, 0);

                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;

                StatusText.text = "Choose one.";
                StopCoroutine("ShuffleText");
                StopCoroutine("FirstCard2With4Shuffle");
            }
            yield return new WaitForSecondsRealtime(0.005f);
            FrameShuffle2++;
        }
    }

    IEnumerator FirstCard3With4Shuffle()
    {
        float UpwardsMovementCard1 = 0.0209f;
        float SidewaysMovementCard1 = 0.011f;
        float UpwardsMovementCard2 = 0.0627f;
        float SidewaysMovementCard2 = 0.011f;
        while (true)
        {
            if (FrameShuffle2 < 22)
            {
                Card3Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 + 0.00099f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.00095f;
                Card4Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 - 0.00099f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.00095f;
            }
            if (FrameShuffle2 >= 22 && FrameShuffle2 < 45)
            {
                Card3Obj.transform.localPosition = new Vector3(UpwardsMovementCard1, 0.0154f, SidewaysMovementCard1);
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.00099f;
                UpwardsMovementCard1 = UpwardsMovementCard1 + 0.00095f;
                Card4Obj.transform.localPosition = new Vector3(UpwardsMovementCard2, 0.0154f, SidewaysMovementCard2);
                SidewaysMovementCard2 = SidewaysMovementCard2 + 0.00099f;
                UpwardsMovementCard2 = UpwardsMovementCard2 - 0.00095f;
            }
            if (FrameShuffle2 == 45)
            {
                FrameShuffle2 = 0;
                FrameShuffle2 = 0;
                CardHighlight1.SetActive(true);
                CardHighlight2.SetActive(true);
                CardHighlight3.SetActive(true);
                CardHighlight4.SetActive(true);
                CardHighlight1.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight2.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight3.transform.localEulerAngles = new Vector3(0, 180, 0);
                CardHighlight4.transform.localEulerAngles = new Vector3(0, 180, 0);

                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;

                StatusText.text = "Choose one.";
                StopCoroutine("ShuffleText");
                StopCoroutine("FirstCard3With4Shuffle");
            }
            yield return new WaitForSecondsRealtime(0.005f);
            FrameShuffle2++;
        }
    }

    protected bool Card1()
    {
        Card1Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Card flipped: 1, Desired: {1}", ModuleID, CorrectCard);
        Card1Sel.OnInteract = InactiveCard;
        Card2Sel.OnInteract = InactiveCard;
        Card3Sel.OnInteract = InactiveCard;
        Card4Sel.OnInteract = InactiveCard;
        if (CorrectCard == 1)
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was correct. You win.", ModuleID);
            StartCoroutine("ShowCard1");
        }
        else
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was incorrect. Strike handed.", ModuleID);
            StartCoroutine("ShowCard1Strike");
        }
        return false;
    }
    protected bool Card2()
    {
        Card2Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Card flipped: 2, Desired: {1}", ModuleID, CorrectCard);
        Card1Sel.OnInteract = InactiveCard;
        Card2Sel.OnInteract = InactiveCard;
        Card3Sel.OnInteract = InactiveCard;
        Card4Sel.OnInteract = InactiveCard;
        if (CorrectCard == 2)
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was correct. You win.", ModuleID);
            StartCoroutine("ShowCard2");
        }
        else
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was incorrect. Strike handed.", ModuleID);
            StartCoroutine("ShowCard2Strike");
        }
        return false;
    }
    protected bool Card3()
    {
        Card3Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Card flipped: 3, Desired: {1}", ModuleID, CorrectCard);
        Card1Sel.OnInteract = InactiveCard;
        Card2Sel.OnInteract = InactiveCard;
        Card3Sel.OnInteract = InactiveCard;
        Card4Sel.OnInteract = InactiveCard;
        if (CorrectCard == 3)
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was correct. You win.", ModuleID);
            StartCoroutine("ShowCard3");
        }
        else
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was incorrect. Strike handed.", ModuleID);
            StartCoroutine("ShowCard3Strike");
        }
        return false;
    }
    protected bool Card4()
    {
        Card4Sel.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Card flipped: 4, Desired: {1}", ModuleID, CorrectCard);
        Card1Sel.OnInteract = InactiveCard;
        Card2Sel.OnInteract = InactiveCard;
        Card3Sel.OnInteract = InactiveCard;
        Card4Sel.OnInteract = InactiveCard;
        if (CorrectCard == 4)
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was correct. You win.", ModuleID);
            StartCoroutine("ShowCard4");
        }
        else
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Card was incorrect. Strike handed.", ModuleID);
            StartCoroutine("ShowCard4Strike");
        }
        return false;
    }

    protected bool InactiveCard()
    {
        return false; 
    }

    IEnumerator ShowCard1()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard1Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Congratulations!";
                StatusText.color = Color.green;
                StartCoroutine("ShowCard2Anim");
                StartCoroutine("ShowCard3Anim");
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 3)
            {
                StatusText.color = Color.white;
                StatusText.text = "Set the payment.";
                StartCoroutine("Payment");
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    IEnumerator ShowCard2()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard2Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Congratulations!";
                StatusText.color = Color.green;
                StartCoroutine("ShowCard1Anim");
                StartCoroutine("ShowCard3Anim");
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 3)
            {
                StatusText.color = Color.white;
                StatusText.text = "Set the payment.";
                StartCoroutine("Payment");
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    IEnumerator ShowCard3()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard3Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Congratulations!";
                StatusText.color = Color.green;
                StartCoroutine("ShowCard1Anim");
                StartCoroutine("ShowCard2Anim");
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 3)
            {
                StatusText.color = Color.white;
                StatusText.text = "Set the payment.";
                StartCoroutine("Payment");
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    IEnumerator ShowCard4()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Congratulations!";
                StatusText.color = Color.green;
                StartCoroutine("ShowCard1Anim");
                StartCoroutine("ShowCard2Anim");
                StartCoroutine("ShowCard3Anim");
            }
            else if (ShowFrame == 3)
            {
                StatusText.color = Color.white;
                StatusText.text = "Set the payment.";
                StartCoroutine("Payment");
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator ShowCard1Strike()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard1Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Too bad!";
                StatusText.color = Color.red;
                StartCoroutine("ShowCard2Anim");
                StartCoroutine("ShowCard3Anim");
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 2)
            {
                StartCoroutine("FoldCard1Anim");
                StartCoroutine("FoldCard2Anim");
                StartCoroutine("FoldCard3Anim");
                StartCoroutine("FoldCard4Anim");
            }
            else if (ShowFrame == 3)
            {
                ShowFrame = 0;
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine("ReturnCard1");
                StopCoroutine("ShowCard1Strike");
                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    IEnumerator ShowCard2Strike()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard2Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Too bad!";
                StatusText.color = Color.red;
                StartCoroutine("ShowCard1Anim");
                StartCoroutine("ShowCard3Anim");
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 3)
            {
                ShowFrame = 0;
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine("ReturnCard1");
                StopCoroutine("ShowCard2Strike");
                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    IEnumerator ShowCard3Strike()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard3Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Too bad!";
                StatusText.color = Color.red;
                StartCoroutine("ShowCard1Anim");
                StartCoroutine("ShowCard2Anim");
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 3)
            {
                ShowFrame = 0;
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine("ReturnCard1");
                StopCoroutine("ShowCard3Strike");
                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
    IEnumerator ShowCard4Strike()
    {
        int ShowFrame = 0;
        while (true)
        {
            if (ShowFrame == 0)
            {
                StartCoroutine("ShowCard4Anim");
            }
            else if (ShowFrame == 1)
            {
                StatusText.text = "Too bad!";
                StatusText.color = Color.red;
                StartCoroutine("ShowCard1Anim");
                StartCoroutine("ShowCard2Anim");
                StartCoroutine("ShowCard3Anim");
            }
            else if (ShowFrame == 3)
            {
                Card1Sel.Highlight.gameObject.SetActive(true);
                Card2Sel.Highlight.gameObject.SetActive(true);
                Card3Sel.Highlight.gameObject.SetActive(true);
                Card4Sel.Highlight.gameObject.SetActive(true);
                ShowFrame = 0;
                GetComponent<KMBombModule>().HandleStrike();
                StartCoroutine("ReturnCard1");
                StopCoroutine("ShowCard4Strike");
                Card1Sel.OnInteract = Card1;
                Card2Sel.OnInteract = Card2;
                Card3Sel.OnInteract = Card3;
                Card4Sel.OnInteract = Card4;
            }
            ShowFrame++;
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }


    IEnumerator ReturnCard1()
    {
        StatusText.text = "Returning...";
        float UpwardsMovementCard1 = 0.011f;
        float SidewaysMovementCard1 = -0.067f;
        while (true)
        {
            if (DealFrame1 < 4)
            {
                Card1Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.023f;
            }
            if (DealFrame1 == 5)
            {
                StartCoroutine("ReturnCard2");
            }
            if (DealFrame1 >= 5 && DealFrame1 < 9)
            {
                Card1Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.001f;
            }
            if (DealFrame1 == 9)
            {
                Card1Obj.transform.localEulerAngles = new Vector3(0, 180, 0);
                Card1Obj.transform.localPosition = new Vector3(-0.067f, 0.007f, -0.061f);
                DealFrame1 = 0;
                StopCoroutine("ReturnCard1");
            }
            DealFrame1++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator ReturnCard2()
    {
        float UpwardsMovementCard1 = 0.011f;
        float SidewaysMovementCard1 = -0.0234f;
        while (true)
        {
            if (DealFrame2 < 4)
            {
                Card2Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.01f;
            }
            if (DealFrame2 == 5)
            {
                StartCoroutine("ReturnCard3");
            }
            if (DealFrame2 >= 5 && DealFrame2 < 9)
            {
                Card2Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.001f;
            }
            if (DealFrame2 == 9)
            {
                Card2Obj.transform.localEulerAngles = new Vector3(0, 180, 0);
                Card2Obj.transform.localPosition = new Vector3(-0.067f, 0.007f, -0.061f);
                DealFrame2 = 0;
                StopCoroutine("ReturnCard2");
            }
            DealFrame2++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator ReturnCard3()
    {
        float UpwardsMovementCard1 = 0.011f;
        float SidewaysMovementCard1 = 0.0216f;
        while (true)
        {
            if (DealFrame3 < 4)
            {
                Card3Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.025f;
            }
            if (DealFrame3 == 5)
            {
                StartCoroutine("ReturnCard4");
            }
            if (DealFrame3 >= 5 && DealFrame3 < 9)
            {
                Card3Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.001f;
            }
            if (DealFrame3 == 9)
            {
                Card3Obj.transform.localEulerAngles = new Vector3(0, 180, 0);
                Card3Obj.transform.localPosition = new Vector3(-0.067f, 0.007f, -0.061f);
                DealFrame3 = 0;
                StopCoroutine("ReturnCard3");
            }
            DealFrame3++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }
    IEnumerator ReturnCard4()
    {
        float UpwardsMovementCard1 = 0.011f;
        float SidewaysMovementCard1 = 0.06659999f;
        while (true)
        {
            if (DealFrame4 < 4)
            {
                Card4Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.023f;
                SidewaysMovementCard1 = SidewaysMovementCard1 - 0.04f;
            }
            if (DealFrame4 >= 5 && DealFrame4 < 9)
            {
                Card4Obj.transform.localPosition = new Vector3(SidewaysMovementCard1, 0.0154f, UpwardsMovementCard1);
                UpwardsMovementCard1 = UpwardsMovementCard1 - 0.001f;
            }
            if (DealFrame4 == 9)
            {
                Card4Obj.transform.localPosition = new Vector3(-0.067f, 0.007f, -0.061f);
                Card4Obj.transform.localEulerAngles = new Vector3(0, 180, 0);
                DealFrame4 = 0;
                DealBtn.gameObject.SetActive(true);
                StartCoroutine("DealText");
                StopCoroutine("ReturnCard4");
            }
            DealFrame4++;
            yield return new WaitForSecondsRealtime(0.005f);
        }
    }


    IEnumerator ShowCard1Anim()
    {
        int Rotation = 180;
        while (true)
        {
            if (FrameCard1 < 10)
            {
                Card1Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard1 = 0;
                StopCoroutine("ShowCard1Anim");
            }
            FrameCard1++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator ShowCard2Anim()
    {
        int Rotation = 180;
        while (true)
        {
            if (FrameCard2 < 10)
            {
                Card2Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard2 = 0;
                StopCoroutine("ShowCard2Anim");
            }
            FrameCard2++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator ShowCard3Anim()
    {
        int Rotation = 180;
        while (true)
        {
            if (FrameCard3 < 10)
            {
                Card3Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard3 = 0;
                StopCoroutine("ShowCard3Anim");
            }
            FrameCard3++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator ShowCard4Anim()
    {
        int Rotation = 180;
        while (true)
        {
            if (FrameCard4 < 10)
            {
                Card4Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard4 = 0;
                StopCoroutine("ShowCard4Anim");
            }
            FrameCard4++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator FoldCard1Anim()
    {
        int Rotation = 0;
        while (true)
        {
            if (FrameCard1 < 11)
            {
                Card1Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard1 = 0;
                StopCoroutine("FoldCard1Anim");
            }
            FrameCard1++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator FoldCard2Anim()
    {
        int Rotation = 0;
        while (true)
        {
            if (FrameCard2 < 11)
            {
                Card2Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard2 = 0;
                StopCoroutine("FoldCard2Anim");
            }
            FrameCard2++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator FoldCard3Anim()
    {
        int Rotation = 0;
        while (true)
        {
            if (FrameCard3 < 11)
            {
                Card3Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {
                FrameCard3 = 0;
                StopCoroutine("FoldCard3Anim");
            }
            FrameCard3++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
    IEnumerator FoldCard4Anim()
    {
        int Rotation = 0;
        while (true)
        {
            if (FrameCard4 < 11)
            {
                Card4Obj.transform.localEulerAngles = new Vector3(0, 180, Rotation);
                Rotation = Rotation - 20;
            }
            else
            {

                Card1Sel.OnInteract = InactiveCard;
                Card2Sel.OnInteract = InactiveCard;
                Card3Sel.OnInteract = InactiveCard;
                Card4Sel.OnInteract = InactiveCard;

                Card1Sel.Highlight.gameObject.SetActive(false);
                Card2Sel.Highlight.gameObject.SetActive(false);
                Card3Sel.Highlight.gameObject.SetActive(false);
                Card4Sel.Highlight.gameObject.SetActive(false);
                FrameCard4 = 0;
                StopCoroutine("FoldCard4Anim");
            }
            FrameCard4++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    IEnumerator Payment()
    {
        int CurrentNumber = 0;
        int Digit = 0;
        PaymentDevice.SetActive(true);
        Card1Obj.SetActive(false);
        Card2Obj.SetActive(false);
        Card3Obj.SetActive(false);
        Card4Obj.SetActive(false);

        StartCoroutine("PaymentDeviceAnim");
        for (int i = 0; i < 10; i++)
        {
            Digit = Random.Range(0, 10);
            CardNumber += Digit.ToString();
            AllCardNumbers[CurrentNumber] = Digit;
            CurrentNumber++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
        MoneyCalulation();
        CardNumberText.text = CardNumber;
        Debug.LogFormat("[Four-Card Monte #{0}] Card Number is {1}", ModuleID, CardNumber);
    }

    void MoneyCalulation()
    {
        int DollarOffset = 0;
        int Batteries = 0;
        int Indicators = 0;
        int Ports = 0;
        //Dollars:
        if (CorrectCoin == 1)
        {
            DollarOffset = AllCoinValues[0];
        }
        else if (CorrectCoin == 2)
        {
            DollarOffset = AllCoinValues[1];
        }
        else if (CorrectCoin == 3)
        {
            DollarOffset = AllCoinValues[2];
        }
        else if (CorrectCoin == 4)
        {
            DollarOffset = AllCoinValues[3];
        }

        
        if (BombInfo.GetBatteryCount() == 0)
        {
            Batteries++;
        }
        else
        {
            Batteries = BombInfo.GetBatteryCount();
        }
        if (BombInfo.GetIndicators().Count() == 0)
        {
            Indicators++;
        }
        else
        {
            Indicators = BombInfo.GetIndicators().Count();
        }
        if (BombInfo.GetPortCount() == 0)
        {
            Ports++;
        }
        else
        {
            Ports = BombInfo.GetPortCount();
        }
        DollarOffset = DollarOffset + Batteries * Indicators * Ports;
        if (DollarOffset > 999)
        {
            DollarOffset = DollarOffset - 1000;
        }
        Debug.LogFormat("[Four-Card Monte #{0}] Desired dollar count: ${1}", ModuleID, DollarOffset);

        DesiredDollars = DollarOffset;
        //Cents:
        DesiredCent1 = AllCardNumbers[BombInfo.GetSerialNumberNumbers().First()];
        DesiredCent2 = AllCardNumbers[BombInfo.GetSerialNumberNumbers().Last()];
        Debug.LogFormat("[Four-Card Monte #{0}] Desired cents count: $0.{1}{2}", ModuleID, DesiredCent1, DesiredCent2);
    }

    IEnumerator PaymentDeviceAnim()
    {
        float SidewaysMovement = 0.0717f;
        int DeviceFrame = 0;
        while (true)
        {
            if (DeviceFrame < 4)
            {
                PaymentDevice.transform.localPosition = new Vector3(SidewaysMovement, 0.0354f, -0.0284f);
                SidewaysMovement = SidewaysMovement - 0.0075f;
            }
            else if (DeviceFrame >= 4 && DeviceFrame < 8)
            {
                PaymentDevice.transform.localPosition = new Vector3(SidewaysMovement, 0.0354f, -0.0284f);
                SidewaysMovement = SidewaysMovement - 0.0035f;
            }
            else if (DeviceFrame == 6)
            {
                StopCoroutine("PaymentDeviceAnim");
            }
            DeviceFrame++;
            yield return new WaitForSecondsRealtime(0.01f);
        }
    }

    protected bool PayDevKey1Press()
    {
        PayDevKey1.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "1";
                DollarText.text = "$" + Dollars;

                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "1";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey2Press()
    {
        PayDevKey2.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "2";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "2";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey3Press()
    {
        PayDevKey3.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "3";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "3";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey4Press()
    {
        PayDevKey4.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "4";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "4";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey5Press()
    {
        PayDevKey5.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "5";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "5";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey6Press()
    {
        PayDevKey6.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "6";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "6";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey7Press()
    {
        PayDevKey7.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "7";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "7";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey8Press()
    {
        PayDevKey8.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "8";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "8";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey9Press()
    {
        PayDevKey9.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "9";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "9";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }
    protected bool PayDevKey0Press()
    {
        PayDevKey0.AddInteractionPunch(0.5f);
        if (!InputtingCents)
        {
            if (CurrentChar <= 3)
            {
                Dollars = Dollars.Substring(Dollars.Length - 2);
                Dollars += "0";
                DollarText.text = "$" + Dollars;
                if (CurrentChar == 3)
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
                    Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
                    InputtingCents = true;
                    CurrentChar = 0;
                }
            }

        }
        else
        {
            if (CurrentChar <= 2)
            {
                Cents = Cents.Substring(Cents.Length - 1);
                Cents += "0";
                CentsText.text = Cents;
               
            }
        }
        CurrentChar++;
        return false;
    }

    protected bool PayDevKeyResetPress()
    {
        PayDevKeyReset.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte #{0}] Reset in order", ModuleID);
        Dollars = "000";
        Cents = "00";
        DollarText.text = "$" + Dollars;
        CentsText.text = Cents;
        CurrentChar = 1;
        InputtingCents = false;
        return false;
    }

    protected bool PayDevKeyCentPress()
    {
        PayDevKeyCents.AddInteractionPunch();
        if (!InputtingCents)
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Now inputting cents.", ModuleID);
            Debug.LogFormat("[Four-Card Monte #{0}] Input: ${1}", ModuleID, int.Parse(Dollars));
            CurrentChar = 1;
            InputtingCents = true;
        }
        else
        {
            Debug.LogFormat("[Four-Card Monte #{0}] Already inputting cents.", ModuleID);
        }
        return false;
    }

    protected bool PayDevKeySubmitPress()
    {
        PayDevKeySubmit.AddInteractionPunch();
        int EnteredDollars = int.Parse(Dollars);
        int EnteredCent1 = int.Parse(Cents.Substring(0, 1));
        int EnteredCent2 = int.Parse(Cents.Substring(1));
        Debug.LogFormat("[Four-Card Monte #{0}] Pressed \"Submit\"", ModuleID);
        Debug.LogFormat("[Four-Card Monte #{0}] Submitted Dollars is ${1}", ModuleID, EnteredDollars);
        Debug.LogFormat("[Four-Card Monte #{0}] Submitted Cents are $0.{1}{2}", ModuleID, EnteredCent1, EnteredCent2);

        if (BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Silly Slots")) == BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Silly Slots")) && BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Poker")) == BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Poker")) && BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Point Of Order")) == BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Point Of Order")) && BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Blackjack")) == BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Blackjack")))
        {
            if (EnteredDollars == DesiredDollars)
            {
                Debug.LogFormat("[Four-Card Monte #{0}] Entered dollars correct. Checking cents...", ModuleID);
                if (EnteredCent1 == DesiredCent1)
                {
                    if (EnteredCent2 == DesiredCent2)
                    {
                        Debug.LogFormat("[Four-Card Monte #{0}] Entered cents correct.", ModuleID);
                        Debug.LogFormat("[Four-Card Monte #{0}] Winnings accepted and sent. Module solved.", ModuleID);
                        TotalEarningsTicket.text = "$" + EnteredDollars.ToString() + "," + EnteredCent1.ToString() + EnteredCent2.ToString();
                        CardNumberTicket.text = CardNumber;
                        int RandomMessage = Random.Range(3, 5);
                        VictoryMessage.text = VictoryMessages[RandomMessage - 1];
                        StartCoroutine("CorrectPayment");
                        StartCoroutine("TicketDispensing");
                        TicketPrinting.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, transform);
                        TicketPrinting.PlaySoundAtTransform("TicketPrinting", transform);
                        GetComponent<KMBombModule>().HandlePass();
                    }
                    else
                    {
                        Debug.LogFormat("[Four-Card Monte #{0}] Entered single cents (${1}) is invalid. (Desired: ${2})", ModuleID, "$0,0" + EnteredDollars, "$0,0" + DesiredDollars);
                        StartCoroutine("WrongPayment");
                        GetComponent<KMBombModule>().HandleStrike();
                    }
                }
                else
                {
                    Debug.LogFormat("[Four-Card Monte #{0}] Entered 10 cents (${1}) is invalid. (Desired: ${2})", ModuleID, "$0," + EnteredDollars + "0", "$0," + DesiredDollars + "0");
                    StartCoroutine("WrongPayment");
                    GetComponent<KMBombModule>().HandleStrike();
                }
            }
            else
            {
                Debug.LogFormat("[Four-Card Monte #{0}] Entered dollars (${1}) is invalid. (Desired: ${2})", ModuleID, EnteredDollars, DesiredDollars);
                StartCoroutine("WrongPayment");
                GetComponent<KMBombModule>().HandleStrike();
            }
        }
        else
        {
            int SolvableCasinoModules = BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Silly Slots")) + BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Poker")) + BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Blackjack")) + BombInfo.GetSolvableModuleNames().Count(x => x.Contains("Point Of Order"));
            int SolvedCasinoModules = BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Silly Slots")) + BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Poker")) + BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Blackjack")) + BombInfo.GetSolvedModuleNames().Count(x => x.Contains("Point Of Order"));
            Debug.LogFormat("<Four-Card Monte {0}> Solvable casino modules: {1}, Solved casino modules: {2}", ModuleID, SolvableCasinoModules, SolvedCasinoModules);
            Debug.LogFormat("[Four-Card Monte #{0}] Cannot deal now: There is still an unsolved casino module.", ModuleID);
            StartCoroutine("CannotSendNow");
            GetComponent<KMBombModule>().HandleStrike();
        }
        return false;
    }

    IEnumerator WrongPayment()
    {
        for (int T = 0; T < 3; T++)
        {
            if (T == 0)
            {
                StatusText.color = Color.red;
                StatusText.text = "Wrong payment!";
            }
            else
            {
                StatusText.color = Color.white;
                StatusText.text = "Set the payment.";
                StopCoroutine("WrongPayment");
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    IEnumerator CannotSendNow()
    {
        for (int T = 0; T < 4; T++)
        {
            if (T == 0)
            {
                StatusText.color = Color.red;
                StatusText.text = "Cannot send now.";
            }
            else if (T == 1)
            {
                StatusText.color = Color.red;
                StatusText.text = "Casino modules";
            }
            else if (T == 2)
            {
                StatusText.color = Color.red;
                StatusText.text = "still unsolved.";
            }
            else
            {
                StatusText.color = Color.white;
                StatusText.text = "Set the payment.";
                StopCoroutine("WrongPayment");
            }
            yield return new WaitForSecondsRealtime(1);
        }
    }

    IEnumerator CorrectPayment()
    {
        for (int T = 0; T < 4; T++)
        {
            if (T == 0)
            {
                StatusText.color = Color.green;
                StatusText.text = "Payment sent!";
            }
            else if (T == 1)
            {
                StatusText.color = Color.white;
                PaymentText.SetActive(false);
                VictoryText.gameObject.SetActive(true);
                StatusText.text = "Shutting down.";
            }
            else
            {
                StatusText.text = "";
                VictoryText.gameObject.SetActive(false);
                StopCoroutine("WrongPayment");
            }
            yield return new WaitForSecondsRealtime(2);
        }
    }

    IEnumerator TicketDispensing()
    {
        int TicketFrame = 0;
        float UpwardsMovement = -0.27f;
        float ForwardsMovement = -2.75f;
        while (true)
        {
            if (TicketFrame < 10)
            {
                Ticket.transform.localPosition = new Vector3(ForwardsMovement, UpwardsMovement, -3.28f);
                UpwardsMovement = UpwardsMovement + 0.1f;
                ForwardsMovement = ForwardsMovement - 1.85f;
            }
            else
            {
                StopCoroutine("TicketDispensing");
            }
            TicketFrame++;
            yield return new WaitForSecondsRealtime(0.25f);
        }
    }

    protected bool ShutdownSequence()
    {
        int RandomMessage = Random.Range(0, 5);
        PayDevKeyShutdown.AddInteractionPunch();
        Debug.LogFormat("[Four-Card Monte] Pressed the power button. {1} Strike handed.", ModuleID, ShutdownMessages[RandomMessage]);
        GetComponent<KMBombModule>().HandleStrike();
        return false;
    }
}
