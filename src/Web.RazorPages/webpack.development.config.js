"use strict";

const webpack = require("webpack");
const path = require("path");
const globImporter = require("node-sass-glob-importer");
const CleanPlugin = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");

module.exports = {
    mode: "development",
    entry: {
        index: "./StaticContent/scripts/index.js",
        admin: "./StaticContent/scripts/admin.js"
    },
    output: {
        path: path.join(__dirname, "wwwroot"),
        filename: "js/[name].bundle.js",
        publicPath: "/"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                include: /StaticContent/,
                exclude: /node_modules/,
                loader: "babel-loader?cacheDirectory=true"
            },
            {
                test: /\.(scss|sass)$/,
                exclude: /node_modules/,
                use: [
                    { loader: "style-loader"},
                    { loader: "css-loader" },
                    {
                        loader: "sass-loader",
                        options: { importer: globImporter() }
                    }
                ]
            },
            {
                test: /\.(jpe?g|png|gif|svg)$/,
                use: {
                    loader: "url-loader",
                    options: {
                        name: "images/[name].[ext]",
                        limit: 10 * 1024
                    }
                }
            },
            {
                test: /\.(woff|woff2|ttf|otf|eot)$/,
                use: {
                    loader: "file-loader",
                    options: { name: "fonts/[name].[ext]" }
                }
            }
        ]
    },
    plugins: [
        // Повторно объявляем jQuery глобальной функцией, чтобы не приходилось
        // импортировать её в каждый пользовательский js-файл.
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            "window.jQuery": "jquery"
        }),
        new webpack.DllReferencePlugin({
            context: ".",
            manifest: require("./wwwroot/lib/vendor-manifest.json"),
            name: "vendor_lib"
        }),
        new CleanPlugin([
            "wwwroot/*.*",
            //"wwwroot/images",
            "wwwroot/fonts",
            "wwwroot/js",
            "wwwroot/css",
        ]),
        new CopyPlugin([
            {
                from: "./StaticContent/images",
                to: "./images"
            },
            {
                from: "./StaticContent/favicon",
                to: "./"
            }
        ])
    ]
};
