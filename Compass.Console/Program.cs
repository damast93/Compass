namespace Compass.Console
{
    using System.IO;
    using System.Threading.Tasks;
    using Compass.Core.Interpreter;
    using Parser;

    class Program
    {
        static async Task Main(string[] args)
        {
            var input = await File.ReadAllTextAsync(args[0]);

            using var renderer = new ImageSharpRenderer(640, 480);

            var scope = new Scope();
            var expressions = Parser.Parse(input);
            var context = new Context(renderer);
            var interpreter = new StatementInterpreter(scope, context);

            foreach (var statement in expressions)
            {
                statement.Accept(interpreter);
            }

            await renderer.SaveToFileAsync("out.png");
        }
    }

}
