using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour
{

    public static KillFeed instance;
    [SerializeField] private GameObject KillListingPrefab;
    [SerializeField] private Sprite[] howImages;
    [SerializeField] private Sprite[] playerImages;

    private void Awake() {
        if (instance != null) {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    // To call this function from another script, use following example code:
    // KillFeed.instance.AddKillListing("killer", "killed");
    public void AddKillListing(string killer, string killed)
    {
        GameObject go = Instantiate(KillListingPrefab, transform);
        go.transform.SetAsFirstSibling();
        KillListing kl = go.GetComponent<KillListing>();
        kl.SetNames(killer, killed);
    }

    // To call this function from another script, use following example code:
    // KillFeed.instance.AddKillListingWithImage("killer", "killed", 0);
    public void AddKillListingWithImages(string killer, string killed, int killerPlayer, int killedPlayer, string attackName)
    {
        GameObject go = Instantiate(KillListingPrefab, transform);
        go.transform.SetAsFirstSibling();
        KillListing kl = go.GetComponent<KillListing>();
        kl.SetNames(killer, killed);
        if (killerPlayer == killedPlayer) {
            kl.SetHowImage(howImages[12]);
        }
        else {
            attackName = attackName.ToLower();
            Debug.Log(attackName);
            Debug.Log(attackName.Contains("leo"));
            if (attackName.Contains("aquarius")) { kl.SetHowImage(howImages[0]); }
            else if (attackName.Contains("aries")) { kl.SetHowImage(howImages[1]); }
            else if (attackName.Contains("cancer")) { kl.SetHowImage(howImages[2]); }
            else if (attackName.Contains("capricorn")) { kl.SetHowImage(howImages[3]); }
            else if (attackName.Contains("gemini")) { kl.SetHowImage(howImages[4]); }
            else if (attackName.Contains("leo")) { kl.SetHowImage(howImages[5]); }
            else if (attackName.Contains("libra")) { kl.SetHowImage(howImages[6]); }
            else if (attackName.Contains("pisces")) { kl.SetHowImage(howImages[7]); }
            else if (attackName.Contains("sagittarius")) { kl.SetHowImage(howImages[8]); }
            else if (attackName.Contains("scorpio")) { kl.SetHowImage(howImages[9]); }
            else if (attackName.Contains("taurus")) { kl.SetHowImage(howImages[10]); }
            else if (attackName.Contains("virgo")) { kl.SetHowImage(howImages[11]); }
            else { kl.SetHowImage(howImages[12]); }
        }
        kl.SetKillerImage(playerImages[killerPlayer]);
        kl.SetKilledImage(playerImages[killedPlayer]);
    }
}
