package sourceafis.matching;

public class BestMatchSkipper
{
    float[][] Collected;

    public BestMatchSkipper(int persons, int skip)
    {
        Collected = new float[skip + 1][];
        for (int i = 0; i < Collected.length; ++i)
        {
            Collected[i] = new float[persons];
            for (int j = 0; j < Collected[i].length; ++j)
                Collected[i][j] = -1;
        }
    }

    public void AddScore(int person, float score)
    {
        for (int nth = Collected.length - 1; nth >= 0; --nth)
        {
            if (Collected[nth][person] < score)
            {
                if (nth + 1 < Collected.length)
                    Collected[nth + 1][person] = Collected[nth][person];
                Collected[nth][person] = score;
            }
        }
    }

    public float GetSkipScore(int person)
    {
        float score = 0;
        for (int nth = Collected.length - 1; nth >= 0; --nth)
            if (Collected[nth][person] > 0)
            {
                score = Collected[nth][person];
                break;
            }
        return score;
    }

    //out person
    public float GetBestScore(int person)
    {
        float best = 0;
        person = -1;
        for (int candidate = 0; candidate < Collected[0].length; ++candidate)
        {
            float score = GetSkipScore(candidate);
            if (score > best)
            {
                best = score;
                person = candidate;
            }
        }
        return best;
    }
}

