var context = require.context('./Client', true, /.+\.spec\.(js|jsx|ts|tsx)$/);
context.keys().forEach(context);

module.exports = context;
