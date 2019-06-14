"use strict";

const webpack = require("webpack");
const path = require("path");
const UglifyJsPlugin = require("uglifyjs-webpack-plugin");
const CleanPlugin = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

module.exports = {
    mode: "production",
    devtool: '',
    entry: {
        vendor: [
            "./StaticContent/lib/bootstrap-customized/css/bootstrap.css",
            "font-awesome/css/font-awesome.css",
            "owl.carousel/dist/assets/owl.carousel.css",
            "jquery/dist/jquery.js",
            "./StaticContent/lib/bootstrap-customized/js/bootstrap.js",
            "owl.carousel/dist/owl.carousel.js",
            "jquery-validation/dist/jquery.validate.js",
            "jquery-validation-unobtrusive/dist/jquery.validate.unobtrusive.js"
        ]
    },
    output: {
        path: path.join(__dirname, "wwwroot", "lib"),
        filename: "js/[name].bundle.min.js",
        publicPath: "../",
        library: "vendor_lib"
    },
    module: {
        rules: [
            {
                test: /\.css$/,
                use: [MiniCssExtractPlugin.loader, "css-loader"]
            },
            {
                test: /\.(jpe?g|png|gif)$/,
                use: {
                    loader: "url-loader",
                    options: {
                        name: "images/[name].[ext]",
                        limit: 10 * 1024
                    }
                }
            },
            {
                test: /\.(woff2?|ttf|otf|eot|svg)$/,
                use: {
                    loader: "file-loader",
                    options: { name: "fonts/[name].[ext]" }
                }
            }
        ]
    },
    plugins: [
        // Twitter Bootstrap и различные плагины jQuery требуют, чтобы функция
        // jQuery была глобальной.
        new webpack.ProvidePlugin({
            $: "jquery",
            jQuery: "jquery",
            "window.jQuery": "jquery"
        }), 
        new MiniCssExtractPlugin({
            filename: "css/[name].bundle.min.css"
        }),
        new CleanPlugin("wwwroot/lib"),
        new webpack.DllPlugin({
            path: path.join(__dirname, "wwwroot", "lib", "vendor-manifest.json"),
            name: "vendor_lib"
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
            new OptimizeCSSAssetsPlugin({})
        ]
    },
};