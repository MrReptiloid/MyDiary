export const dateToString = (input: Date) => {
  const datetime = new Date(input)
  const day = datetime.getDay()
  const month = datetime.getMonth()
  const year = datetime.getFullYear()
  return `${day}.${month}.${year}`
}