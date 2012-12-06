/**
 * @author Veaceslav Dubenco
 * @since 08.10.2012
 */
package sourceafis.general;

import java.util.Collection;
import java.util.Iterator;
import java.util.List;
import java.util.ListIterator;

/**
 * 
 */
public class RectangleC implements List<Point> {

	public int X;
	public int Y;
	public int Width;
	public int Height;

	public int getLeft() {
		return X;
	}

	public void setLeft(int value) {
		Width += X - value;
		X = value;
	}

	public int getBottom() {
		return Y;
	}

	public void setBottom(int value) {
		Height += Y - value;
		Y = value;
	}

	public int getRight() {
		return X + Width;
	}

	public void setRight(int value) {
		Width = value - X;
	}

	public int getTop() {
		return Y + Height;
	}

	public void setTop(int value) {
		Height = value - Y;
	}

	public Point getPoint() {
		return new Point(getLeft(), getBottom());
	}

	public void setPoint(Point value) {
		X = value.X;
		Y = value.Y;
	}

	public Size getSize() {
		return new Size(Width, Height);
	}

	public void setSize(Size value) {
		Width = value.Width;
		Height = value.Height;
	}

	public Range getRangeX() {
		return new Range(getLeft(), getRight());
	}

	public Range getRangeY() {
		return new Range(getBottom(), getTop());
	}

	public Point getCenter() {
		return new Point((getRight() + getLeft()) / 2,
				(getBottom() + getTop()) / 2);
	}

	public int getTotalArea() {
		return Width * Height;
	}

	public RectangleC(RectangleC other) {
		X = other.X;
		Y = other.Y;
		Width = other.Width;
		Height = other.Height;
	}

	public RectangleC(Point at, Size size) {
		X = at.X;
		Y = at.Y;
		Width = size.Width;
		Height = size.Height;
	}

	public RectangleC(int x, int y, int width, int height) {
		X = x;
		Y = y;
		Width = width;
		Height = height;
	}

	public RectangleC(Point begin, Point end) {
		X = begin.X;
		Y = begin.Y;
		Width = end.X - begin.X;
		Height = end.Y - begin.Y;
	}

	public RectangleC(Size size) {
		X = 0;
		Y = 0;
		Width = size.Width;
		Height = size.Height;
	}

	public RectangleC(int width, int height) {
		X = 0;
		Y = 0;
		Width = width;
		Height = height;
	}

	public boolean Contains(Point point) {
		return point.X >= getLeft() && point.Y >= getBottom()
				&& point.X < getRight() && point.Y < getTop();
	}

	public Point GetRelative(Point absolute) {
		return new Point(absolute.X - X, absolute.Y - Y);
	}

	public PointF GetFraction(Point absolute) {
		Point relative = GetRelative(absolute);
		return new PointF(relative.X / (float) Width, relative.Y
				/ (float) Height);
	}

	public void Shift(Point relative) {
		setPoint(Calc.Add(getPoint(), relative));
	}

	public RectangleC GetShifted(Point relative) {
		RectangleC result = new RectangleC(this);
		result.Shift(relative);
		return result;
	}

	public void Clip(RectangleC other) {
		if (getLeft() < other.getLeft())
			setLeft(other.getLeft());
		if (getRight() > other.getRight())
			setRight(other.getRight());
		if (getBottom() < other.getBottom())
			setBottom(other.getBottom());
		if (getTop() > other.getTop())
			setTop(other.getTop());
	}

	public void Include(Point point) {
		if (point == null) {
			point = new Point(0, 0);
		}
		if (getLeft() > point.X)
			setLeft(point.X);
		if (getRight() <= point.X)
			setRight(point.X + 1);
		if (getBottom() > point.Y)
			setBottom(point.Y);
		if (getTop() <= point.Y)
			setTop(point.Y + 1);
	}

	public Iterator<Point> getIterator() {
		return new Iterator<Point>() {
			private Point point = new Point(getLeft(), getBottom());

			@Override
			public boolean hasNext() {
				return point.Y < getTop() && point.X < getRight();
			}

			@Override
			public sourceafis.general.Point next() {
				Point retVal = new Point(point.X, point.Y);
				if (point.X >= getRight() - 1) {
					point.X = getLeft();
					point.Y++;
				} else {
					point.X++;
				}
				return retVal;

			}

			@Override
			public void remove() {
				throw new UnsupportedOperationException();
			}
		};
	}

	public boolean isReadOnly() {
		return true;
	}

	public int getCount() {
		return getTotalArea();
	}

	@Override
	public Point get(int at) {
		return new Point(at % Width, at / Width);
	}

	/* (non-Javadoc)
	 * @see java.util.List#add(java.lang.Object)
	 */
	@Override
	public boolean add(Point arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#add(int, java.lang.Object)
	 */
	@Override
	public void add(int arg0, Point arg1) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#addAll(java.util.Collection)
	 */
	@Override
	public boolean addAll(Collection<? extends Point> arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#addAll(int, java.util.Collection)
	 */
	@Override
	public boolean addAll(int arg0, Collection<? extends Point> arg1) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#clear()
	 */
	@Override
	public void clear() {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#contains(java.lang.Object)
	 */
	@Override
	public boolean contains(Object obj) {
		if (obj instanceof Point) {
			return Contains((Point) obj);
		} else {
			return false;
		}
	}

	/* (non-Javadoc)
	 * @see java.util.List#containsAll(java.util.Collection)
	 */
	@Override
	public boolean containsAll(Collection<?> arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#indexOf(java.lang.Object)
	 */
	@Override
	public int indexOf(Object arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#isEmpty()
	 */
	@Override
	public boolean isEmpty() {
		// TODO Auto-generated method stub
		return false;
	}

	/* (non-Javadoc)
	 * @see java.util.List#iterator()
	 */
	@Override
	public Iterator<Point> iterator() {
		// TODO Auto-generated method stub
		return getIterator();
	}

	/* (non-Javadoc)
	 * @see java.util.List#lastIndexOf(java.lang.Object)
	 */
	@Override
	public int lastIndexOf(Object arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#listIterator()
	 */
	@Override
	public ListIterator<Point> listIterator() {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#listIterator(int)
	 */
	@Override
	public ListIterator<Point> listIterator(int arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#remove(java.lang.Object)
	 */
	@Override
	public boolean remove(Object arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#remove(int)
	 */
	@Override
	public Point remove(int arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#removeAll(java.util.Collection)
	 */
	@Override
	public boolean removeAll(Collection<?> arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#retainAll(java.util.Collection)
	 */
	@Override
	public boolean retainAll(Collection<?> arg0) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#set(int, java.lang.Object)
	 */
	@Override
	public Point set(int arg0, Point arg1) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#size()
	 */
	@Override
	public int size() {
		return getCount();
	}

	/* (non-Javadoc)
	 * @see java.util.List#subList(int, int)
	 */
	@Override
	public List<Point> subList(int arg0, int arg1) {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#toArray()
	 */
	@Override
	public Object[] toArray() {
		throw new UnsupportedOperationException();
	}

	/* (non-Javadoc)
	 * @see java.util.List#toArray(T[])
	 */
	@Override
	public <T> T[] toArray(T[] arg0) {
		throw new UnsupportedOperationException();
	}

}
