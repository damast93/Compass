using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;
using Compass.Core.Interpreter;

namespace Compass.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var command = new RootCommand("Renders the given Compass script and outputs the rendering as an image file.") {
                new Argument<FileInfo>(
                    "input-file",
                    "The input file to process."
                ) {
                    Arity = ArgumentArity.ExactlyOne,
                },
                new Option<FileInfo>(
                    new [] { "--output-file", "-o"},
                    () => new FileInfo("out.png"),
                    "The output file to render to."
                ),
                new Option<int>(
                    new[] {"--width", "-w"},
                    () => 640,
                    "The width of the image to render."
                ),
                new Option<int>(
                    new[] {"--height", "-h"},
                    () => 480,
                    "The height of the image to render."
                ),
            };

            command.Handler = CommandHandler.Create<FileInfo, FileInfo, int, int>(RenderScriptAsync);

            await command.InvokeAsync(args);
        }

        private static async Task RenderScriptAsync(
            FileInfo inputFile,
            FileInfo outputFile,
            int width,
            int height
        ) {
            var input = await File.ReadAllTextAsync(inputFile.FullName);

            using var renderer = new ImageSharpRenderer(width, height);

            var scope = new Scope();
            var expressions = Parser.Parser.Parse(input);
            var context = new Context(renderer);
            var interpreter = new StatementInterpreter(scope, context);

            foreach (var statement in expressions)
            {
                statement.Accept(interpreter);
            }

            await renderer.SaveToFileAsync(outputFile.FullName);
        }
    }
}
