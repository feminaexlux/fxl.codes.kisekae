const path = require('path');

module.exports = [{
    mode: "development",
    entry: {
        main: "./Scripts/main.ts",
        style: "./Styles/main.scss"
    },
    output: {
        // This is necessary for webpack to compile
        // But we never use style-bundle.js
        filename: 'js/[name].js',
        path: path.resolve(__dirname, 'wwwroot'),
    },
    resolve: {
        // Add `.ts` and `.tsx` as a resolvable extension.
        extensions: [".ts", ".tsx", ".js"]
    },
    module: {
        rules: [
            {
                test: /\.scss$/,
                use: [
                    {
                        loader: 'file-loader',
                        options: {
                            name: 'css/main.css',
                        },
                    },
                    {loader: 'extract-loader'},
                    {loader: 'css-loader'},
                    {
                        loader: 'sass-loader',
                        options: {
                            // Prefer Dart Sass
                            implementation: require('sass'),

                            // See https://github.com/webpack-contrib/sass-loader/issues/804
                            webpackImporter: false,
                            sassOptions: {
                                includePaths: ['./node_modules'],
                                sourceMap: true
                            },
                            sourceMap: true
                        },
                    },
                ]
            },
            {test: /\.tsx?$/, loader: "ts-loader"}
        ]
    },
    stats: {
        // Display bailout reasons
        optimizationBailout: true,
    },
}];