package sourceafis.matching;

import java.util.Arrays;
import java.util.Comparator;
import sourceafis.general.Calc;

public class BestMatchSkipper
{
    float[][] collected;
    public class PersonsSkipScore{
    	public int person;
    	public float score;
    }
    
    public BestMatchSkipper(int persons, int skip)
    {
        collected = new float[skip + 1][];
        for (int i = 0; i < collected.length; ++i)
        {
            collected[i] = new float[persons];
            for (int j = 0; j < collected[i].length; ++j)
                collected[i][j] = -1;
        }
    }

    public void AddScore(int person, float score)
    {
        for (int nth = collected.length - 1; nth >= 0; --nth)
        {
            if (collected[nth][person] < score)
            {
                if (nth + 1 < collected.length)
                    collected[nth + 1][person] = collected[nth][person];
                collected[nth][person] = score;
            }
        }
    }

    public float GetSkipScore(int person)
    {
        float score = 0;
        for (int nth = collected.length - 1; nth >= 0; --nth)
            if (collected[nth][person] > 0)
            {
                score = collected[nth][person];
                break;
            }
        return score;
    }
   /*
    * Unused method??
    */
    public PersonsSkipScore[] getSortedScores()
    {
        PersonsSkipScore[] results = new PersonsSkipScore[collected[0].length];
        for (int person = 0; person < results.length; ++person)
        {
            results[person].person = person;
            results[person].score = GetSkipScore(person);
        }
        
        //Array.Sort(results, (left, right) => Calc.Compare(right.Score, left.Score));
         Arrays.sort(results,new Comparator<PersonsSkipScore>() {
			public int compare(PersonsSkipScore left, PersonsSkipScore right) {
			  return	Calc.Compare(right.score, left.score);
		      }
		   });
        return results;
    }
    //out person
    /*public float GetBestScore(int person)
    {
        float best = 0;
        person = -1;
        for (int candidate = 0; candidate < collected[0].length; ++candidate)
        {
            float score = GetSkipScore(candidate);
            if (score > best)
            {
                best = score;
                person = candidate;
            }
        }
        return best;
    }*/
}