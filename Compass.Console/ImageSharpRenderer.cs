using System;
using Compass.Core.Display;
using Compass.Core.Geometry;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Compass.Core.Maths;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using ImageSharp = SixLabors.ImageSharp;
using Point = Compass.Core.Geometry.Point;

namespace Compass.Console
{
    public class ImageSharpRenderer : DisplayEngine, IDisposable
    {
        private const float PointThickness = 4f;

        private static readonly Font Font = SystemFonts.Find("Arial").CreateFont(12);
        private static readonly ImageSharp.PointF NameOffset = new(6, -6);

        private readonly ImageSharp.Image<Rgba32> _image;
        private readonly Segment[] _imageBounds;

        public ImageSharpRenderer(int width, int height)
        {
            this._image = new ImageSharp.Image<Rgba32>(width, height);
            this._imageBounds = GetImageBounds(width, height);
        }

        private static Segment[] GetImageBounds(int width, int height)
        {
            var northWest = new Point(0, 0);
            var northEast = new Point(width, 0);
            var southWest = new Point(0, height);
            var southEast = new Point(width, height);

            return new[]
            {
                new Segment(northWest, northEast),
                new Segment(northEast, southEast),
                new Segment(southEast, southWest),
                new Segment(southWest, northWest),
            };
        }

        public async Task SaveToFileAsync(string fileName)
        {
            await using var stream = File.OpenWrite(fileName);
            await _image.SaveAsync(stream, new PngEncoder());
        }

        public override void Render(Point point)
        {
            var polygon = new EllipsePolygon(ConvertPoint(point), PointThickness);

            this._image.Mutate(
                context => context.Fill(ImageSharp.Color.Navy, polygon)
            );
        }

        public override void Render(NamedPoint point)
        {
            this.Render((Point)point);

            this._image.Mutate(
                context => context.DrawText(
                    point.Name,
                    Font, ImageSharp.Color.Black,
                    ConvertPoint(point) + NameOffset
                )
            );
        }

        public override void Render(Circle circle)
        {
            var polygon = new EllipsePolygon(
                ConvertPoint(circle.Center),
                (float)circle.Radius
            );

            this._image.Mutate(
                context => context.Draw(ImageSharp.Color.Black, 1f, polygon)
            );
        }

        public override void Render(Line ray)
        {
            var endpoints = this.GetLineEndPoints(ray);
            this._image.Mutate(
                context => context.DrawPolygon(
                    ImageSharp.Color.Gray,
                    1f,
                    endpoints[0],
                    endpoints[1]
                )
            );
        }

        public override void Render(Segment segment)
        {
            this._image.Mutate(
                context => context.DrawPolygon(
                    ImageSharp.Color.Red,
                    1f,
                    ConvertPoint(segment.A),
                    ConvertPoint(segment.B)
                )
            );
        }

        public override void Render(Ray ray)
        {
            var endpoints = this.GetLineEndPoints(ray);
            this._image.Mutate(
                context => context.DrawPolygon(
                    ImageSharp.Color.Gray  ,
                    1f,
                    ConvertPoint(ray.A),
                    endpoints[0]
                )    
            );
        }

        private ImageSharp.PointF[] GetLineEndPoints(Line line)
        {
            var points = new List<Point>();
            foreach (var segment in this._imageBounds)
            {
                var intersection = segment.Intersect(line).Pick(1).SingleOrDefault();
                if (intersection != null && !points.Any(x => intersection.DistanceTo(x) <= MathGlobals.EPS))
                {
                    points.Add(intersection);
                }
            }

            return points.Select(ConvertPoint).ToArray();
        }

        private static ImageSharp.PointF ConvertPoint(Point point)
            => new(
                (float)point.x,
                (float)point.y
            );

        public void Dispose()
        {
            _image.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
