const path = require('path');
const webpack = require('webpack');

const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const CheckerPlugin = require("awesome-typescript-loader").CheckerPlugin;
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");
const BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;

const bundleOutputDir = './wwwroot/dist';

module.exports = (env) => {
  const isDevBuild = !(env && env.prod);
  const isAnalyze = env && env.analyze;

  const cssLoader = {
    loader: 'css-loader',
    options: {
      modules: {
        localIdentName: '[name]__[local]___[hash:base64:5]'
      },
      importLoaders: 1,
      sourceMap: true
    }
  };
  return {
    stats: { modules: false },
    entry: {
      'root': './Client/root.tsx',
      'order': './Client/order.tsx',
      'react': ['react', 'react-dom', 'react-router', 'react-toolbox', 'react-bootstrap', 'react-datepicker'],
      'showdown': ['showdown']
    },
    resolve: { extensions: ['.js', '.jsx', '.ts', '.tsx'] },
    output: {
      path: path.join(__dirname, bundleOutputDir),
      filename: '[name].js',
      publicPath: '/dist/'
    },
    devServer: {
      clientLogLevel: 'info',
      compress: true,
      port: process.env.DEV_SERVER_PORT || 8080,
      injectClient: false,
      // transportMode: 'ws',  // TODO: move to WS once it's no longer experimental
      contentBase: path.resolve(__dirname, './dist'),
      hot: true,
      inline: true
    },
    mode: isDevBuild ? "development" : "production",
    devtool: isDevBuild ? "eval-source-map" : "source-map",
    module: {
      rules: [
        {
          test: /\.(js|jsx|ts|tsx)$/,
          include: /Client/,
          exclude: /node_modules/,
          use: 'awesome-typescript-loader?silent=true'
        },
        {
          test: /\.css$/,
          use: [
            !isDevBuild ? MiniCssExtractPlugin.loader : { loader: "style-loader" },
            cssLoader
          ]
        },
        {
          test: /\.scss$/,
          use: [
            !isDevBuild ? MiniCssExtractPlugin.loader : { loader: "style-loader" },
            { loader: 'css-loader', options: { sourceMap: true } },
            { loader: "sass-loader", options: { sourceMap: true } }
          ]
        },
        {
          test: /\.(png|jpg|jpeg|gif|woff)$/,
          use: 'url-loader?limit=25000'
        },
        {
          test: /\.svg$/,
          include: /Client/,
          exclude: /node_modules/,
          use: [
            {
              loader: 'babel-loader',
              options: { plugins: ["@babel/plugin-proposal-object-rest-spread"] }
            },
            {
              loader: "react-svg-loader",
              options: {
                jsx: true,
                svgo: {
                  plugins: [
                    { removeTitle: false }
                  ],
                  floatPrecision: 2
                }
              }
            }
          ]
        },
      ]
    },
    optimization: {
      minimizer: isDevBuild ? [] : [
        new TerserPlugin({
          cache: true,
          parallel: true,
          sourceMap: true,
        }),
        new OptimizeCssAssetsPlugin({}),
      ],
      splitChunks: {
        chunks: 'all',
        cacheGroups: {
          default: false,
          vendor: {
            name: 'vendor',
            test: /[\\/]node_modules[\\/]/,
            priority: -10
          },
        }
      }
    },
    plugins: [
      new CheckerPlugin(),
      ...isDevBuild ? [
        // Plugins that apply in development builds only
      ] : [
          // Plugins that apply in production builds only
          new MiniCssExtractPlugin({
            filename: '[name].css',
          }),
        ],
      // Webpack Bundle Analyzer
      // https://github.com/th0r/webpack-bundle-analyzer
      ...isAnalyze ? [new BundleAnalyzerPlugin()] : [],
    ]
  };
};
