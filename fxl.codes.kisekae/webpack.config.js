const path = require('path');

module.exports = [{
    mode: "development",
    entry: {
        main: ["./Scripts/play.ts"]
    },
    output: {
        filename: 'js/[name].js',
        path: path.resolve(__dirname, 'wwwroot'),
    },
    resolve: {
        // Add `.ts` and `.tsx` as a resolvable extension.
        extensions: [".ts", ".tsx", ".js"]
    },
    module: {
        rules: [
            {test: /\.tsx?$/, loader: "ts-loader"}
        ]
    },
    stats: {
        errorDetails: true
    }
}]