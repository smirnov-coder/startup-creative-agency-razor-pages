"use strict";

const webpack = require("webpack");
const path = require("path");
const globImporter = require("node-sass-glob-importer");
const CleanPlugin = require("clean-webpack-plugin");
const CopyPlugin = require("copy-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");

const ImageminPlugin = require("imagemin-webpack");
const imageminGifsicle = require("imagemin-gifsicle");
const imageminSvgo = require("imagemin-svgo");
const imageminMozjpeg = require("imagemin-mozjpeg");
const imageminPngquant = require("imagemin-pngquant");

module.exports = {
    mode: "production",
    devtool: '', // Removed dev-tools mapping
    entry: {
        index: "./StaticContent/scripts/index.js",
        admin: "./StaticContent/scripts/admin.js"
    },
    output: {
        path: path.join(__dirname, "wwwroot"),
        filename: "js/[name].bundle.min.js",
        publicPath: "../"
    },
    module: {
        rules: [
            {
                test: /\.js$/,
                include: /StaticContent/,
                exclude: /node_modules/,
                loader: "babel-loader"
            },
            {
                test: /\.(scss|sass)$/,
                exclude: /node_modules/,
                use: [
                    MiniCssExtractPlugin.loader,
                    { loader: "css-loader" },
                    {
                        loader: "postcss-loader",
                        options: {
                            ident: "postcss",
                            plugins: [
                                require("autoprefixer")(),
                                require("css-mqpacker")(),
                                require("cssnano")()
                            ]
                        }
                    },
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
            },
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
            "wwwroot/js",
            "wwwroot/css",
            "wwwroot/images",
            "wwwroot/fonts"
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
        ]),
        // ImageminPlugin должен идти после всех плагинов, взаимодействующих с изображениями.
        new ImageminPlugin({
            test: /\.(jpe?g|png|gif|svg)$/,
            include: /images/, // Оптимизировать картинки только в папке wwwroot/images.
            name: "images/[name].[ext]",
            bail: false,
            cache: true,
            imageminOptions: {
                plugins: [
                    imageminGifsicle({
                        interlaced: false
                    }),
                    imageminMozjpeg({
                        progressive: true,
                        quality: 80
                    }),
                    imageminPngquant({
                        quality: [0.8, 0.8],
                        speed: 4
                    }),
                    imageminSvgo({
                        removeViewBox: true
                    })
                ]
            }
        }),
        new MiniCssExtractPlugin({
            filename: "css/[name].bundle.min.css"
        })
    ],
    optimization: {
        concatenateModules: true,
        minimizer: [
            new UglifyJsPlugin({
                parallel: true,
                sourceMap: true,
                uglifyOptions: {
                    mangle: true,
                    compress: {
                        warnings: false,
                        pure_getters: true,
                        unsafe: true,
                        unsafe_comps: true,
                        conditionals: true,
                        unused: true,
                        comparisons: true,
                        sequences: true,
                        dead_code: true,
                        evaluate: true,
                        if_return: true,
                        join_vars: true
                    },
                    output: {
                        comments: false,
                    }
                }
            }),
        ]
    },
};