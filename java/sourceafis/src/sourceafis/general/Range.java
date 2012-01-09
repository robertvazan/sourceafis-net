package sourceafis.general;


public class Range{
	public Range(){
		
	}
    public Range(int begin, int end)
    {
        Begin = begin;
        End = end;
    }

    public Range(int length)
    {
        Begin = 0;
        End = length;
    }

    public int getLength() {
    	 return End - Begin; 
    }

    public int Begin;
    public int End;

    public int Interpolate(int index, int count)
    {
        return Calc.Interpolate(index, count, getLength()) + Begin;
    }
}