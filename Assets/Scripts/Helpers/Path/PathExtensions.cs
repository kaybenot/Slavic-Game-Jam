using Data.Path;

namespace Helpers.Path {
    public static class PathExtensions {

        public static Path PathFromSpline(this in SplinePathData data, bool invert) {
            return new Path() {
                start = invert ? data.second : data.first,
                end = invert ? data.first : data.second
            };
        }
    }
}