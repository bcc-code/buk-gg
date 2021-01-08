const BundleAnalyzerPlugin = require('webpack-bundle-analyzer')
    .BundleAnalyzerPlugin;
const webpack = require('webpack');

// vue.config.js
module.exports = {
    outputDir: 'wwwroot',
    chainWebpack: (config) => {
        // Remove the HotModuleReplacementPlugin plugin if site is running as an ASP.Net Core application
        // This is to avoid hot reloading getting added twice, which causes errors.
        if (process.env.IsAspNetServer === 'True') {
            config.plugins.delete('hmr');
        }

        config
            .plugin('creo-replace-moment')
            .use(webpack.ContextReplacementPlugin, [
                /moment[/\\]locale$/,
                /(en-gb|de|nb)/,
            ]);
    },
    configureWebpack: {
        devtool: 'source-map',
    },
};
