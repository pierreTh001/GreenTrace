/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{ts,tsx}"],
  theme: {
    extend: {
      colors: {
        brand: {
          50: "#effdf6",
          100: "#d9faeb",
          200: "#b1f2d7",
          300: "#7de7bf",
          400: "#3cd5a1",
          500: "#10b981",
          600: "#0c9467",
          700: "#0a7555",
          800: "#0a5c45",
          900: "#083f30"
        }
      }
    },
  },
  plugins: [],
}
