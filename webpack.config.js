const path = require('path');

module.exports = [{
    entry: {
        main: ["./Scripts/main.ts", "./Styles/main.scss"],
        uploader: "./Scripts/upload.ts",
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
                    {
                        loader: 'css-loader',
                        options: {
                            // What the **fuck**
                            // https://github.com/peerigon/extract-loader/issues/111#issuecomment-948791524
                            esModule: false
                        }
                    },
                    {
                        loader: 'sass-loader',
                        options: {
                            // Prefer Dart Sass
                            implementation: require('sass'),

                            // See https://github.com/webpack-contrib/sass-loader/issues/804
                            webpackImporter: false,
                            sassOptions: {
                                includePaths: ['./node_modules'],
                            },
                        },
                    }
                ],
            },
            {test: /\.tsx?$/, loader: "ts-loader"}
        ],
    },
    stats: {
        errorDetails: true
    }
}]