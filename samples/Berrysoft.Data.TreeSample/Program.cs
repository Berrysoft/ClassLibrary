using System;
using System.Linq;

namespace Berrysoft.Data.TreeSample
{
    class Program
    {
        static BinaryTree<int> tree;
        static BinaryTree<int> current;
        static void Main(string[] args)
        {
            tree = new BinaryTree<int>();
            Console.WriteLine("How many groups do you want to test: ");
            int n = int.Parse(Console.ReadLine());
            while (n-- > 0)
            {
                Console.WriteLine("Please enter the values by pre order, splited by space:");
                int[] front = Console.ReadLine().Split(' ').Select(str => int.Parse(str)).ToArray();
                Console.WriteLine("Please enter the values by in order, splited by space:");
                int[] mid = Console.ReadLine().Split(' ').Select(str => int.Parse(str)).ToArray();
                if (front.Length != mid.Length)
                {
                    Console.WriteLine("The length of the two arrays should be equal.");
                    continue;
                }
                tree.LeftChild = null;
                tree.RightChild = null;
                current = tree;
                Create(front, mid);
                Console.WriteLine("The post order of this tree is:");
                Console.WriteLine(string.Join(" ", tree.AsPostOrderEnumerable().Select(node => node.ToString()).ToArray()));
                Console.WriteLine("The level order of this tree is:");
                Console.WriteLine(string.Join(" ", tree.AsLevelOrderEnumerable().Select(node => node.ToString()).ToArray()));
            }
        }
        static void Create(in Span<int> front, in Span<int> mid)
        {
            int n = front.Length;
            BinaryTree<int> tr = current;
            tr.Value = front[0];
            int i;
            for (i = 0; i < n; i++)
            {
                if (mid[i] == front[0])
                {
                    break;
                }
            }
            if (i > 0)
            {
                current = (tr.LeftChild = new BinaryTree<int>());
                Create(front.Slice(1, i), mid);
            }
            if (n - 1 - i > 0)
            {
                current = (tr.RightChild = new BinaryTree<int>());
                Create(front.Slice(i + 1, n - 1 - i), mid.Slice(i + 1, n - 1 - i));
            }
        }
    }
}
