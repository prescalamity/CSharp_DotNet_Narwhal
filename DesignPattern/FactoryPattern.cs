using DesignPattern;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesignPattern
{


	#region===================================== 简单工厂 =============================================

	/// <summary>
	/// 抽象产品类，也可以是非抽象
	/// </summary>
	public class Product
	{
		public virtual void use() { }
	}

	/// <summary>
	/// 具体产品类A
	/// </summary>
	public class ConcreteProductA : Product
	{
		public override void use()
		{
			Console.WriteLine($"Using ConcreteProductA");
		}
	}

	/// <summary>
	/// 具体产品类B
	/// </summary>
	public class ConcreteProductB : Product
	{
		public override void use()
		{
			Console.WriteLine($"Using ConcreteProductB");

		}
	}

	/// <summary>
	/// 简单工厂类，处理对象的创建操作，复用一些类似对象的创建时需要的共同代码，同时也方便统一管理创建操作需要增加一些操作的情况
	/// </summary>
	public class SimpleFactory
	{

		public Product createProduct(string type)
		{
			if (type == "A")
			{
				return new ConcreteProductA();
			}
			else if (type == "B")
			{
				return new ConcreteProductB();
			}
			else
			{
				// 处理未知产品类型的情况
				return null;
			}
		}

	}

	public class FactoryPattern
	{

		int main()
		{
			// 使用简单工厂创建产品
			SimpleFactory factory = new SimpleFactory();

			ConcreteProductA productA = factory.createProduct("A") as ConcreteProductA;
			if (productA != null)
			{
				productA.use();
			}

			ConcreteProductB productB = factory.createProduct("B") as ConcreteProductB;
			if (productB != null)
			{
				productB.use();
			}

			Product unknownProduct = factory.createProduct("C");
			if (unknownProduct == null)
			{
				Console.WriteLine($"Unknown product type.");

			}

			return 0;
		}


	}

	#endregion===================================== 简单工厂 =============================================




	#region===================================== 抽线复杂工厂 =============================================

	// 华为手机和电脑
	// 场景描述：我联系了一家中国工厂，品尝该厂的苹果和香蕉，吃完后又找到一家美国工厂，品尝他家的苹果和香蕉，对比下口感。

	// 抽象产品A 
	class AbstractProductA
	{
		public virtual void use() { }
	}

	// 具体产品A1
	class ConcreteProductA1 :  AbstractProductA {
		public override void use()  
		{
			Console.WriteLine($"Using ConcreteProductA1");
		}
	}

	// 具体产品A2
	class ConcreteProductA2 : AbstractProductA
	{
		public override void use() 
		{
			Console.WriteLine($"Using ConcreteProductA2");
		}
	}



	// 抽象产品B
	class AbstractProductB
	{
		public virtual void eat() { }
	}

	// 具体产品B1
	class ConcreteProductB1 : AbstractProductB
	{
		public override void eat()
		{
			Console.WriteLine($"Eating ConcreteProductB1");

		}
	}

	// 具体产品B2
	class ConcreteProductB2 : AbstractProductB
	{
		public override void eat()
		{
			Console.WriteLine($"Eating ConcreteProductB2");

		}
	}



	// 抽象工厂
	abstract class AbstractFactory
	{
		public abstract AbstractProductA createProductA() ;
		public abstract AbstractProductB createProductB() ;
	}

	// 具体工厂1，只能产生A族中的一个产品，和B族中的一个产品，例如：中国厂商生产的苹果和香蕉，口感和价格
	class ConcreteFactory1 : AbstractFactory
	{
		public override AbstractProductA createProductA() 
		{
			return new ConcreteProductA1();     // 该工厂只能生产某一种形状，A1，
			//return new ConcreteProductA2();
		}

		public override AbstractProductB createProductB()
		{
			return new ConcreteProductB1();     // 该工厂只能生产某一种颜色，B1
			//return new ConcreteProductB2();
		}
	}

	// 具体工厂2，只能产生A族中的一个产品，和B族中的一个产品，例如：美国厂商生产的苹果和香蕉，口感和价格
	class ConcreteFactory2 :  AbstractFactory
	{
		public override AbstractProductA createProductA() 
		{
			//return new ConcreteProductA1();
			return new ConcreteProductA2();
		}

		public override AbstractProductB createProductB()
		{
			//return new ConcreteProductB1();
			return new ConcreteProductB2();
		}
	}

	public class AbstractFactoryPattern { 
		int main()
		{
			// 使用具体工厂1创建产品族1
			AbstractFactory factory1 = new ConcreteFactory1();          // 美国工厂
			AbstractProductA productA1 = factory1.createProductA();		// 香蕉
			AbstractProductB productB1 = factory1.createProductB();		// 苹果

			productA1.use();
			productB1.eat();

			// 使用具体工厂2创建产品族2
			AbstractFactory factory2 = new ConcreteFactory2();			// 中国工厂
			AbstractProductA productA2 = factory2.createProductA();
			AbstractProductB productB2 = factory2.createProductB();     

			productA2.use();
			productB2.eat();

			return 0;
		}
	}




	// ------------------------------------------- 案例 二 ---------------------------------------------------




	public interface Shape
	{
		void draw();
	}

	public class Rectangle : Shape
	{
		public void draw()
		{
			Console.WriteLine("Inside Rectangle::draw() method.");
		}
	}

	public class Square : Shape
	{	
		public void draw()
		{
			Console.WriteLine("Inside Square::draw() method.");
		}
	}

	public class Circle : Shape
	{
	   public void draw()
		{
			Console.WriteLine("Inside Circle::draw() method.");
		}
	}


	/// <summary>
	/// 接口
	/// </summary>
	public interface Color
	{
		void fill();
	}

	public class Red : Color
	{
		public void fill()
		{
			Console.WriteLine("Inside Red::fill() method.");
		}
	}

	public class Green : Color
	{
		public void fill()
		{
			Console.WriteLine("Inside Green::fill() method.");
		}
	}

	public class Blue : Color
	{
		public void fill()
		{
			Console.WriteLine("Inside Blue::fill() method.");
		}
	}

	/// <summary>
	/// 抽象类
	/// </summary>
	public abstract class AbstractFactory2
	{
		public abstract Color getColor(string color);
		public abstract Shape getShape(string shape);
	}


	public class ShapeFactory : AbstractFactory2
	{
		public override Color getColor(string color)
		{
			return null;
		}

		public override Shape getShape(string shapeType)
		{
			if (shapeType == null)
			{
				return null;
			}
			if (shapeType.Equals ("CIRCLE"))
			{
				return new Circle();
			}
			else if (shapeType.Equals("RECTANGLE"))
			{
				return new Rectangle();
			}
			else if (shapeType.Equals("SQUARE"))
			{
				return new Square();
			}
			return null;
		}


	}

	public class ColorFactory : AbstractFactory2
	{
		public override Color getColor(string color)
		{
			if (color == null)
			{
				return null;
			}
			if (color.Equals("RED"))
			{
				return new Red();
			}
			else if (color.Equals("GREEN"))
			{
				return new Green();
			}
			else if (color.Equals("BLUE"))
			{
				return new Blue();
			}
			return null;
		}

		public override Shape getShape(string shapeType)
		{
			return null;
		}


	}

	/// <summary>
	/// 创建一个工厂创造器/生成器类，通过传递形状或颜色信息来获取工厂。
	/// </summary>
	public class FactoryProducer
	{
		public static AbstractFactory2 getFactory(string choice)
		{
			if (choice.Equals("SHAPE"))
			{
				return new ShapeFactory();
			}
			else if (choice.Equals("COLOR"))
			{
				return new ColorFactory();
			}
			return null;
		}
	}

	/// <summary>
	/// 使用 FactoryProducer 来获取 AbstractFactory，通过传递类型信息来获取实体类的对象。
	/// </summary>
	public class AbstractFactoryPatternDemo
	{
		public static void main(string[] args)
		{

			//获取形状工厂
			AbstractFactory2 shapeFactory = FactoryProducer.getFactory("SHAPE");

			//获取形状为 Circle 的对象
			Shape shape1 = shapeFactory.getShape("CIRCLE");

			//调用 Circle 的 draw 方法
			shape1.draw();

			//获取形状为 Rectangle 的对象
			Shape shape2 = shapeFactory.getShape("RECTANGLE");

			//调用 Rectangle 的 draw 方法
			shape2.draw();

			//获取形状为 Square 的对象
			Shape shape3 = shapeFactory.getShape("SQUARE");

			//调用 Square 的 draw 方法
			shape3.draw();

			//获取颜色工厂
			AbstractFactory2 colorFactory = FactoryProducer.getFactory("COLOR");

			//获取颜色为 Red 的对象
			Color color1 = colorFactory.getColor("RED");

			//调用 Red 的 fill 方法
			color1.fill();

			//获取颜色为 Green 的对象
			Color color2 = colorFactory.getColor("GREEN");

			//调用 Green 的 fill 方法
			color2.fill();

			//获取颜色为 Blue 的对象
			Color color3 = colorFactory.getColor("BLUE");

			//调用 Blue 的 fill 方法
			color3.fill();
		}
	}


	#endregion===================================== 抽线复杂工厂 =============================================



}
