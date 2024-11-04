const colors = require('tailwindcss/colors')
const withMT = require("@material-tailwind/react/utils/withMT");

module.exports = withMT({
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  darkMode: 'media', // or 'media' or 'class'
  theme: {
    extend: {
      colors: {
        primary: colors.blue,
        'layout-primary': '#173BA0',
        'layout-bg': colors.slate,
        'layout-text': colors.white,
        main: colors.gray[100],
        'main-text': colors.gray[900],
      },
      screens: {
        'sm': '640px',
        'md': '768px',  // iPad Mini Breakpoint
        'lg': '1024px',
        'xl': '1280px',
      },
      fontSize: {
        'xs': '0.75rem',
        'sm': '0.875rem',
        'base': '1rem',
        'lg': '1.125rem',
        'xl': '1.25rem',
      },
    },
    fontFamily: {
      sans: ['Arial', 'sans-serif']
    }
  },
  plugins: [],
})

