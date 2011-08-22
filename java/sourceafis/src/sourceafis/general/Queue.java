package sourceafis.general;
/**
 * Array-based implementation of the queue.
 */
public class Queue<E>  
{
    private E[ ] theArray;
    private int        currentSize;
    private int        front;
    private int        back;

    private static final int DEFAULT_CAPACITY = 10;
    /**
     * Construct the queue.
     */
    public Queue( )
    {
        theArray = newElementArray(DEFAULT_CAPACITY);
        makeEmpty( );
    }

    private E[] newElementArray(int capacity) {
    	 return (E[]) new Object[capacity];
    }

    /**
     * Test if the queue is logically empty.
     * @return true if empty, false otherwise.
     */
    public boolean isEmpty( )
    {
        return currentSize == 0;
    }

    /**
     * Make the queue logically empty.
     */
    public void makeEmpty( )
    {
        currentSize = 0;
        front = 0;
        back = -1;
    }

    /**
     * Return and remove the least recently inserted item
     * from the queue.
     * @return the least recently inserted item in the queue.
     * @throws UnderflowException if the queue is empty.
     */
    public E dequeue( )
    {
        if( isEmpty( ) )
            throw new UnderflowException( "ArrayQueue dequeue" );
        currentSize--;

        E returnValue = theArray[ front ];
        front = increment( front );
        return returnValue;
    }
    
    /**
     * Get the least recently inserted item in the queue.
     * Does not alter the queue.
     * @return the least recently inserted item in the queue.
     * @throws UnderflowException if the queue is empty.
     */
    public E getFront( )
    {
        if( isEmpty( ) )
            throw new UnderflowException( "ArrayQueue getFront" );
        return theArray[ front ];
    }

    /**
     * Insert a new item into the queue.
     * @param x the item to insert.
     */
    public void enqueue(E x )
    {
        if( currentSize == theArray.length )
            doubleQueue( );
        back = increment( back );
        theArray[ back ] = x;
        currentSize++;
    }

    /**
     * Internal method to increment with wraparound.
     * @param x any index in theArray's range.
     * @return x+1, or 0 if x is at the end of theArray.
     */
    private int increment( int x )
    {
        if( ++x == theArray.length )
            x = 0;
        return x;
    }
    
    /**
     * Internal method to expand theArray.
     */
    private void doubleQueue( )
    {
        E[] newArray;

        newArray = newElementArray(theArray.length * 2);

            // Copy elements that are logically in the queue
        for( int i = 0; i < currentSize; i++, front = increment( front ) )
            newArray[ i ] = theArray[ front ];

        theArray = newArray;
        front = 0;
        back = currentSize - 1;
    }


    
    
}

